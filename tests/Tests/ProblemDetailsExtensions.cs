using Shouldly;

namespace Tests
{
    public static class ProblemDetailsExtensions
    {
        public static void Check(this Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails, string errorMessage)
        {
            if (errorMessage == null)
            {
                problemDetails.ShouldBeSuccessful();
            }
            else
            {
                problemDetails.ShouldThrowError(errorMessage);
            }
        }

        public static void ShouldBeSuccessful(this Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails)
        {
            problemDetails.ShouldBeNull();
        }

        public static void ShouldThrowError(this Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails, string errorMessage)
        {
            problemDetails.ShouldNotBeNull();

            problemDetails.Detail.ShouldBe(errorMessage);
        }
    }
}