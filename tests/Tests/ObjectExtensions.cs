using System.Net;
using Shouldly;

namespace Tests
{
    public static class ObjectExtensions
    {
        public static void Check<TResponse>(this (HttpStatusCode StatusCode, TResponse? Response, Microsoft.AspNetCore.Mvc.ProblemDetails? Error) result, string? errorMessage, Action<TResponse>? assert = null) where TResponse : class
        {
            if (errorMessage == null)
            {
                result.ShouldBeSuccessful();

                assert?.Invoke(result.Response!);
            }
            else
            {
                result.ShouldThrowError(errorMessage);

            }
        }

        public static void ShouldBeSuccessful<TResponse>(this (HttpStatusCode StatusCode, TResponse? Response, Microsoft.AspNetCore.Mvc.ProblemDetails? Error) result) where TResponse : class
        {
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Error.ShouldBeNull();
            result.Response.ShouldNotBeNull();
        }

        public static void ShouldThrowError<TResponse>(this (HttpStatusCode StatusCode, TResponse? Response, Microsoft.AspNetCore.Mvc.ProblemDetails? Error) result, string errorMessage) where TResponse : class
        {
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            result.Error!.ShouldThrowError(errorMessage);
            result.Response.ShouldBeNull();
        }
    }
}