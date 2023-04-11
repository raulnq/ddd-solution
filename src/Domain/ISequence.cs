namespace Domain
{
    public interface ISequence
    {
        Task<int> GetNextValue<T>();

        Task<long> GetNextLongValue<T>();
    }
}