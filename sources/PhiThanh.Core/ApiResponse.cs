using System.Net;

namespace PhiThanh.Core
{
    public class ApiResponse(int statusCode, string message, bool success)
    {
        public ApiResponse() : this(HttpStatusCode.OK.GetHashCode(), nameof(HttpStatusCode.OK), true)
        {
        }

        public int StatusCode { get; set; } = statusCode;
        public string Message { get; set; } = message;
        public bool Success { get; set; } = success;
        public IEnumerable<ValidationErrorResponse> Errors { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public ApiResponse(int statusCode, string message, T result, bool success)
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
            Success = success;
        }

        public ApiResponse(T result) : this(HttpStatusCode.OK.GetHashCode(), nameof(HttpStatusCode.OK), result, true)
        {
        }

        public ApiResponse() : this(default!)
        {
        }

        public T Result { get; set; }
    }

    public class ValidationErrorResponse
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string PropertyName { get; set; }
    }
}
