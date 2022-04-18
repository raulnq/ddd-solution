using Domain;

namespace Application
{
    public class ValidationErrorDetail
    {
        public string Code { get; set; }

        public object[]? Parameters { get; set; }

        public ValidationErrorDetail(string code, object[]? parameters)
        {
            Code = code;

            Parameters = parameters;
        }

    }

    public class ValidationException : BaseException
    {
        public ValidationException(Dictionary<string, IEnumerable<ValidationErrorDetail>> errors)
            : base("ValidationErrorDetail") => Errors = errors;

        public IDictionary<string, IEnumerable<ValidationErrorDetail>> Errors { get; }
    }
}
