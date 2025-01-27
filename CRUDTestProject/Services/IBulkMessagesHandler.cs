namespace CRUDTestProject.Services
{
    public interface IBulkMessagesHandler
    {
        void RenameUser(string oldUsername, string newUsername);
        void DeleteUserMessages(string username);
    }
}
