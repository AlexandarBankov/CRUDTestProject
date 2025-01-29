namespace Management.Middleware.Exceptions
{
    public class ApiCallFailedException(string message) : Exception(message)
    {
    }
}
