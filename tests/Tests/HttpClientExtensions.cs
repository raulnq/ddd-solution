using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace Tests
{
    public static class HttpClientExtensions
    {
        public static async Task<(HttpStatusCode StatusCode, TResponse? Response, Microsoft.AspNetCore.Mvc.ProblemDetails? Error)> Post<TRequest, TResponse>(this HttpClient client, string requestUri, TRequest request)
            where TResponse : class
        {
            var requestbody = JsonSerializer.Serialize(request);

            var httpResponse = await client.PostAsync(requestUri, new StringContent(requestbody, Encoding.Default, "application/json"));

            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

            options.Converters.Add(new JsonStringEnumConverter());

            var responseBody = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = JsonSerializer.Deserialize<TResponse>(responseBody, options);

                return (httpResponse.StatusCode, response, null);
            }
            else
            {
                var error = JsonSerializer.Deserialize<Microsoft.AspNetCore.Mvc.ProblemDetails>(responseBody, options);

                return (httpResponse.StatusCode, default(TResponse), error);
            }
        }

        public static async Task<(HttpStatusCode StatusCode, TResponse? Response, Microsoft.AspNetCore.Mvc.ProblemDetails? Error)> Get<TResponse>(this HttpClient client, string requestUri)
            where TResponse : class
        {
            var httpResponse = await client.GetAsync(requestUri);

            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

            options.Converters.Add(new JsonStringEnumConverter());

            var responseBody = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = JsonSerializer.Deserialize<TResponse>(responseBody, options);

                return (httpResponse.StatusCode, response, null);
            }
            else
            {
                var error = JsonSerializer.Deserialize<Microsoft.AspNetCore.Mvc.ProblemDetails>(responseBody, options);

                return (httpResponse.StatusCode, default(TResponse), error);
            }
        }
    }
}