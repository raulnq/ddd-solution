﻿using System.Net;
using System.Web;

namespace Tests
{
    public class HttpDriver : Driver
    {
        public readonly IHttpClienFactory _factory;

        public HttpDriver(IHttpClienFactory factory)
        {
            _factory = factory;
        }

        public Task<(HttpStatusCode StatusCode, TResponse? Response, Microsoft.AspNetCore.Mvc.ProblemDetails? Error)> Post<TRequest, TResponse>(string requestUri, TRequest request)
            where TResponse : class
        {
            var client = _factory.CreateClient();

            return client.Post<TRequest, TResponse>(requestUri, request);
        }

        public Task<(HttpStatusCode StatusCode, Microsoft.AspNetCore.Mvc.ProblemDetails? Error)> Post<TRequest>(string requestUri, TRequest request)
        {
            var client = _factory.CreateClient();

            return client.Post<TRequest>(requestUri, request);
        }

        public Task<(HttpStatusCode StatusCode, TResponse? Response, Microsoft.AspNetCore.Mvc.ProblemDetails? Error)> Put<TRequest, TResponse>(string requestUri, TRequest request)
            where TResponse : class
        {
            var client = _factory.CreateClient();

            return client.Put<TRequest, TResponse>(requestUri, request);
        }

        public Task<(HttpStatusCode StatusCode, Microsoft.AspNetCore.Mvc.ProblemDetails? Error)> Put<TRequest>(string requestUri, TRequest request)
        {
            var client = _factory.CreateClient();

            return client.Put<TRequest>(requestUri, request);
        }

        public Task<(HttpStatusCode StatusCode, Microsoft.AspNetCore.Mvc.ProblemDetails? Error)> Delete(string requestUri)
        {
            var client = _factory.CreateClient();

            return client.Delete(requestUri);
        }

        public Task<(HttpStatusCode StatusCode, TResponse? Response, Microsoft.AspNetCore.Mvc.ProblemDetails? Error)> Get<TRequest, TResponse>(string requestUri, TRequest request)
            where TResponse : class
        {
            var client = _factory.CreateClient();

            var uriBuilder = new UriBuilder($"host/{requestUri.TrimStart('/')}");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var param in typeof(TRequest).GetProperties())
            {
                var value = param.GetValue(request)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    query[param.Name] = value;
                }
            }

            uriBuilder.Query = query.ToString();

            return client.Get<TResponse>(uriBuilder.Uri.PathAndQuery);
        }

        public record EmptyRequest();

        public record EmptyResponse();
    }
}