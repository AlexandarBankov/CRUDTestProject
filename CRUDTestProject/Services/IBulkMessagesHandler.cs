namespace CRUDTestProject.Services
{
    public interface IBulkMessagesHandler
    {
        Task RenameUser(string oldUsername, string newUsername);
        Task DeleteUserMessages(string username);
    }
}
