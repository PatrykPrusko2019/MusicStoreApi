namespace MusicStoreApi.Exceptions
{
    public class DuplicateValueException : Exception
    {
        public DuplicateValueException(string message) : base(message)
        {
        }
    }
}
