namespace DocuTest.Shared.Interfaces
{
    public interface IDataStrategy<T>
    {
        string Expression();

        bool Allows(T value);
    }
}
