using CRUDTestProject.Data.Repositories;

namespace CRUDTestProject.Services.Implementation
{
    public class BulkMessagesHandler(IBulkMessagesRepository repository, ILogger<BulkMessagesHandler> logger) : IBulkMessagesHandler
    {
        public async Task DeleteUserMessages(string username)
        {
            logger.LogInformation($"Deleting the messages of {username}");

            await repository.DeleteUserMessages(username);
        }

        public async Task RenameUser(string oldUsername, string newUsername)
        {
            logger.LogInformation($"Renaming {oldUsername} to {newUsername}");

            repository.RenameUser(oldUsername, newUsername);
        }
    }
}
