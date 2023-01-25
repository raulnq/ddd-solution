namespace Domain
{
    public interface IClock
    {
        DateTimeOffset Now { get; }
    }
}