using RestSharp;
using RestSharp.Authenticators;
using System.Net;

namespace PhiThanh.Core.Providers
{
    public class HttpClientResult<T>
    {
        public T? Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string? ErrorException { get; set; }
        public string? Content { get; set; }
    }

    public class HttpBadRequestResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public bool Success { get; set; }
    }

    public class HttpMockupClientResult<T>
    {
        public T Result { get; set; }
    }

    public interface IHttpClientProvider
    {
        Task<HttpClientResult<T?>> PostAsync<T>(string url, string route, object payload);
        Task<HttpClientResult<T?>> PostAsync<T>(string url, string route, object payload, string username, string password);
        Task<HttpClientResult<T?>> PostAsync<T>(string url, string route, object payload, string token);

        HttpClientResult<T?> Post<T>(string url, string route, object payload);
        HttpClientResult<T?> Post<T>(string url, string route, object payload, string username, string password);
        HttpClientResult<T?> Post<T>(string url, string route, object payload, string token);
    }

    public class HttpClientProvider : IHttpClientProvider
    {
        public async Task<HttpClientResult<T?>> PostAsync<T>(string url, string route, object payload)
        {
            var options = new RestClientOptions(url);
            return await InternalPostAsync<T>(options, route, payload);
        }

        public async Task<HttpClientResult<T?>> PostAsync<T>(string url, string route, object payload, string username, string password)
        {
            var options = new RestClientOptions(url)
            {
                Authenticator = new HttpBasicAuthenticator(username, password)
            };
            return await InternalPostAsync<T>(options, route, payload);
        }

        public async Task<HttpClientResult<T?>> PostAsync<T>(string url, string route, object payload, string token)
        {
            var authenticator = new JwtAuthenticator(token);
            var options = new RestClientOptions(url)
            {
                Authenticator = authenticator
            };
            return await InternalPostAsync<T>(options, route, payload);
        }

        public HttpClientResult<T?> Post<T>(string url, string route, object payload)
        {
            var options = new RestClientOptions(url);
            return InternalPost<T>(options, route, payload);
        }

        public HttpClientResult<T?> Post<T>(string url, string route, object payload, string username, string password)
        {
            var options = new RestClientOptions(url)
            {
                Authenticator = new HttpBasicAuthenticator(username, password)
            };
            return InternalPost<T>(options, route, payload);
        }

        public HttpClientResult<T?> Post<T>(string url, string route, object payload, string token)
        {
            var authenticator = new JwtAuthenticator(token);
            var options = new RestClientOptions(url)
            {
                Authenticator = authenticator
            };
            return InternalPost<T>(options, route, payload);
        }

        private static async Task<HttpClientResult<T?>> InternalPostAsync<T>(RestClientOptions options, string route, object payload)
        {
            var client = new RestClient(options);
            var request = new RestRequest(route, Method.Post).AddJsonBody(payload);
            request.AddHeader("Content-Type", "application/json");
            var response = await client.ExecuteAsync<T?>(request);

            return new HttpClientResult<T?>
            {
                StatusCode = response.StatusCode,
                Data = response.Data,
                ErrorException = response.ErrorException?.Message,
                Content = response.Content
            };
        }

        private static HttpClientResult<T?> InternalPost<T>(RestClientOptions options, string route, object payload)
        {
            var client = new RestClient(options);
            var request = new RestRequest(route, Method.Post).AddJsonBody(payload);
            request.AddHeader("Content-Type", "application/json");
            var response = client.Execute<T?>(request);

            return new HttpClientResult<T?>
            {
                StatusCode = response.StatusCode,
                Data = response.Data,
                ErrorException = response.ErrorException?.Message,
                Content = response.Content
            };
        }
    }
}
