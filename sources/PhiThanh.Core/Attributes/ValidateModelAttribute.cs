using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PhiThanh.Core.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                throw new ValidationException(context.ModelState.Values.SelectMany(s => s.Errors.Select(e => new ValidationFailure(string.Empty, e.ErrorMessage))).ToList());
            }
        }
    }
}
