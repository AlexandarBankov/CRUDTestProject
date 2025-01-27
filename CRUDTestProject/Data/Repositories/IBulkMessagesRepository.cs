namespace CRUDTestProject.Data.Repositories
{
    public interface IBulkMessagesRepository
    {
        void RenameUser(string oldUsername, string newUsername);
        void DeleteUserMessages(string username);
    }
}
