using FluentValidation;
using FluentValidation.Results;

namespace PhiThanh.Core.Utils
{
    public static class ExceptionUtils
    {
        public static void ThrowValidation(string property, string errorCode, string errorMessage)
        {
            var validation = new ValidationFailure(property, errorMessage)
            {
                ErrorCode = errorCode
            };
            throw new ValidationException(new List<ValidationFailure> { validation });
        }

        public static void ThrowInternalError(string message)
        {
            throw new InvalidOperationException(message);
        }
    }
}
