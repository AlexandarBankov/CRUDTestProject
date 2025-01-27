using CRUDTestProject.Data.Repositories;

namespace CRUDTestProject.Services
{
    public class BulkMessagesHandler(IBulkMessagesRepository repository, ILogger<BulkMessagesHandler> logger) : IBulkMessagesHandler
    {
        public void DeleteUserMessages(string username)
        {
            logger.LogInformation($"Deleting the messages of {username}");

            repository.DeleteUserMessages(username);
        }

        public void RenameUser(string oldUsername, string newUsername)
        {
            logger.LogInformation($"Renaming {oldUsername} to {newUsername}");

            repository.RenameUser(oldUsername, newUsername);
        }
    }
}
