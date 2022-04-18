namespace Domain
{
    public class DomainException : BaseException
    {
        public DomainException(string code, params object[] parameters) : base(code, parameters)
        {
        }

        public DomainException(string code, string description) : base(code, description)
        {
        }
    }
}