namespace Management.Middleware.Exceptions
{
    public class NoSuchUserException(string message) : Exception(message)
    {
    }
}
