using Domain;

namespace Infrastructure
{
    public class InfrastructureException : BaseException
    {
        public InfrastructureException(string code, object[] parameters) : base(code, parameters)
        {
        }

        public InfrastructureException(string code, string description) : base(code, description)
        {
        }

        public InfrastructureException(string code) : base(code)
        {
        }
    }
}