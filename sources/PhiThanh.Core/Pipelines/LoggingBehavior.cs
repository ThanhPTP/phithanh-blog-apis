using MediatR;
using System.Diagnostics;
using System.Text.Json;

namespace PhiThanh.Core.Pipelines
{
    public class LoggingBehavior<TRequest, TResponse>(Serilog.ILogger logger) :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Serilog.ILogger _logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = request.GetType().Name;
            TResponse response;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                response = await next();
            }
            finally
            {
                stopwatch.Stop();
                _logger.Information($"[{requestName}]: request: {JsonSerializer.Serialize(request)}, Execution time={stopwatch.ElapsedMilliseconds}ms");
            }

            return response;
        }
    }
}
