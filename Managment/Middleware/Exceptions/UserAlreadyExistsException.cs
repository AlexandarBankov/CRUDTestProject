namespace Management.Middleware.Exceptions
{
    public class UserAlreadyExistsException(string message) : Exception(message)
    {
    }
}
