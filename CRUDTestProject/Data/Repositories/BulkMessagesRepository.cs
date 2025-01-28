using Microsoft.EntityFrameworkCore;

namespace CRUDTestProject.Data.Repositories
{
    public class BulkMessagesRepository(ApplicationDbContext dbContext) : IBulkMessagesRepository
    {
        public void DeleteUserMessages(string username)
        {
            dbContext.RemoveRange(dbContext.Messages.Where(m => m.Username == username));
            _ = dbContext.SaveChangesAsync().Result;
        }

        public void RenameUser(string oldUsername, string newUsername)
        {
            var messages = dbContext.Messages.Where(m => m.Username == oldUsername);

            foreach (var message in messages)
            {
                message.Username = newUsername;
            }

            dbContext.SaveChanges();
        }
    }
}
