using Autofac;
using PhiThanh.DataAccess;
using PhiThanh.DataAccess.Kernel;

namespace PhiThanh.WebApi.Modules
{
    public class RepositoryModule : Module
    {
        /// <summary>
        /// Using except to remove business you unwanted to register
        /// .Where(t => t.Name.EndsWith("Repository"))
        /// .Except<MyUnwantedRepository>();
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterGeneric(typeof(GenericRepository<>))
                .As(typeof(IGenericRepository<>))
                .InstancePerRequest();

            builder.RegisterAssemblyTypes(typeof(GenericRepository<>).Assembly)
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces();
        }
    }
}
