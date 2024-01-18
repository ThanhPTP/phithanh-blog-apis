using MediatR;
using MediatR.Pipeline;
using Newtonsoft.Json;
using System.Net;

namespace PhiThanh.Core.Pipelines
{
    public class ValidationExceptionHandler<TRequest, TResponse, TException>(Serilog.ILogger logger) : IRequestExceptionHandler<TRequest, TResponse, TException>
         where TRequest : IRequest<TResponse>
         where TResponse : ApiResponse, new()
         where TException : FluentValidation.ValidationException
    {
        private readonly Serilog.ILogger _logger = logger;

        public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
        {
            var requestName = request.GetType().Name;
            var requestGuid = Guid.NewGuid().ToString();

            var response = new TResponse
            {
                StatusCode = HttpStatusCode.BadRequest.GetHashCode(),
                Message = exception.Message,
                Success = false,
                Errors = exception.Errors
                .Select(gs => new ValidationErrorResponse
                {
                    ErrorCode = gs.ErrorCode,
                    PropertyName = gs.PropertyName,
                    ErrorMessage = gs.ErrorMessage
                })
            };

            _logger.Error(exception, "[ERR_ID = {@requestGuid}] Validation errors while handling request of type {@requestName}: {@responseJson}",
                requestGuid, requestName, JsonConvert.SerializeObject(response));

            state.SetHandled(response!);
            return Task.FromException<FluentValidation.ValidationException>(exception);
        }
    }
}
