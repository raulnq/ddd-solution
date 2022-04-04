using Domain;

namespace Application
{
    public class ApplicationException : BaseException
    {
        public ApplicationException(string code) : base(code)
        {
        }

        public ApplicationException(string code, params object[] parameters) : base(code, parameters)
        {
        }

        public ApplicationException(string code, string description) : base(code, description)
        {
        }
    }
}
