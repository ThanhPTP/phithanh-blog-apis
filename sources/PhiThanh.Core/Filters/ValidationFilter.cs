using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace PhiThanh.Core.Filters
{
    public class ValidationFilter<T> : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            T? argToValidate = context.GetArgument<T>(1);
            IValidator<T>? validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

            if (validator is not null)
            {
                var validationResult = await validator.ValidateAsync(argToValidate!);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors.ConvertAll(e => new ValidationFailure(string.Empty, e.ErrorMessage)));
                }
            }

            return await next.Invoke(context);
        }
    }
}
