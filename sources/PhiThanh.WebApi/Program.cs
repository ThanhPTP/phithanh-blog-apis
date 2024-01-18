using Asp.Versioning;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Carter;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using PhiThanh.Core;
using PhiThanh.Core.Middlewares;
using PhiThanh.Core.Extensions;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PhiThanh.DataAccess.Contexts;
using PhiThanh.Core.Pipelines;
using PhiThanh.Modules.Kernel;
using PhiThanh.WebApi.Modules;
using PhiThanh.DataAccess;

namespace PhiThanh.WebApi
{
    public static class Program
    {
        private static readonly bool IsDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Configuration(builder.Configuration);
            ConfigureServices(builder.Services, builder.Configuration);
            ConfigurePipelines(builder.Services);
            ConfigureHostBuilder(builder.Host);

            var app = builder.Build();
            ConfigureApp(app);

            LogStatisticsServer(app);
            app.Run();
        }

        private static void LogStatisticsServer(WebApplication app)
        {
            var serviceTotals = app.Services.GetAutofacRoot()
                .ComponentRegistry
                .Registrations
                .Count();

            var bitServer = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            Log.Logger.Information($"START SERVER AT: {DateTime.UtcNow}, os: {Environment.OSVersion} {bitServer}, name: {Environment.MachineName}, processors: {Environment.ProcessorCount}, services registrations: {serviceTotals}");
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddRazorPages();
            services.AddCarter();
            services.AddControllersWithViews();
            services
                .AddControllers(options => options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider()))
                .ConfigureApiBehaviorOptions(o =>
                {
                    o.SuppressInferBindingSourcesForParameters = true;
                    o.SuppressModelStateInvalidFilter = true;
                })
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opt.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";
                });

            services.AddEndpointsApiExplorer();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(AutoMapperProfile).GetTypeInfo().Assembly));
            services.AddHttpContextAccessor();
            services.AddHttpContext();
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
            Version version = Assembly.GetEntryAssembly()!.GetName().Version!;
            AddSwagger(services, $"API - {version} - {DateTime.UtcNow}");
            services
                .AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", (JwtBearerOptions options) =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = AppSettings.Instance.Issuer,
                        ValidAudience = AppSettings.Instance.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.Instance.TokenSecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services
                .AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("Bearer")
                        .Build());

            services.AddRolePermissionAuthorize();
            services.AddMinioBlobStorage(configuration);
            services.AddAutoMapper(typeof(Program).Assembly);

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = pi => pi.GetMethod?.IsPublic == true;
                cfg.AddMaps(new[] { typeof(AutoMapperProfile).Assembly });
            });

            IMapper mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);

            var serverVersion = new MySqlServerVersion(new Version(AppSettings.Instance.Persistence.Major,
                                   AppSettings.Instance.Persistence.Minor,
                                   AppSettings.Instance.Persistence.Build));

            services.AddDbContext<DbContext, CoreDataContext>(p => p.UseMySql(AppSettings.Instance.Persistence.CoreDataContext, serverVersion)
                                                        .EnableSensitiveDataLogging());

            services.AddCors(o =>
            {
                o.AddPolicy("AllowSetOrigins", options =>
                {
                    options.AllowAnyHeader();
                    options.AllowAnyMethod();
                    options.SetIsOriginAllowed(_ => true);
                    options.AllowCredentials();
                });
            });

            services.Configure<KestrelServerOptions>(options => options.Limits.MaxRequestBodySize = int.MaxValue);

            if (!IsDevelopment)
            {
                mapperConfiguration.CompileMappings();
            }
        }

        private static void ConfigurePipelines(IServiceCollection services)
        {
            services.AddSingleton<ExceptionHandlingMiddleware>();
            services.AddValidatorsFromAssembly(typeof(BaseValidator<>).Assembly);
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));
            services.AddSingleton(typeof(IRequestExceptionHandler<,,>), typeof(ValidationExceptionHandler<,,>));
        }

        private static void ConfigureApp(WebApplication app)
        {
            app.UsePathBase(AppSettings.Instance.BaseUrl);
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseCors("AllowSetOrigins");
            app.UseHttpContext();
            if (!IsDevelopment)
            {
                app.UseHttpsRedirection();
            }
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages();
            app.UseSwagger(c => c.PreSerializeFilters.Add((swaggerDoc, _)
                => swaggerDoc.Servers.Add(new OpenApiServer() { Url = AppSettings.Instance.BaseUrl })));
            app.UseSwaggerUI(c =>
            {
                c.DefaultModelsExpandDepth(-1);
                c.SwaggerEndpoint(AppSettings.Instance.BaseUrl + "/swagger/v1/swagger.json", "PhiThanh.WebApi v1");
            });
            app.MapCarter();
        }

        private static void Configuration(ConfigurationManager configuration)
        {
            configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                optional: true, reloadOnChange: true);
            AppSettings.Instance = configuration.GetSection("AppSettings").Get<AppSettings>()!;

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationId()
                .Enrich.WithProperty("@source", "phithanh-api");

            Log.Logger = loggerConfig
               .WriteTo.Console()
               .CreateLogger();
        }

        private static void ConfigureHostBuilder(ConfigureHostBuilder host)
        {
            host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                containerBuilder.RegisterInstance(Log.Logger);
                containerBuilder.RegisterModule<RepositoryModule>();
                containerBuilder.RegisterType<CoreDapperContext>().As<IDapperContext>()
                    .WithParameter("connection", AppSettings.Instance.Persistence.CoreDataContext!)
                    .InstancePerLifetimeScope();
            });
        }

        private static void AddSwagger(IServiceCollection services, string title)
        {
            services.AddSwaggerGen((SwaggerGenOptions c) =>
            {
                c.CustomSchemaIds(type => type.ToString());
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = title,
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.ResolveConflictingActions((IEnumerable<ApiDescription> r) => r.First());
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                } });
            });
        }
    }
}
