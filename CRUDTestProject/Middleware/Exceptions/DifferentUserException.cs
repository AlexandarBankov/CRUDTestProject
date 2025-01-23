namespace CRUDTestProject.Middleware.Exceptions
{
    public class DifferentUserException(string message) : Exception(message)
    {
    }
}
