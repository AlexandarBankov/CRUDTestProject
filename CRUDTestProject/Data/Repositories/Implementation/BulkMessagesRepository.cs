namespace CRUDTestProject.Data.Repositories.Implementation
{
    public class BulkMessagesRepository(ApplicationDbContext dbContext) : IBulkMessagesRepository
    {
        public async Task DeleteUserMessages(string username)
        {
            dbContext.RemoveRange(dbContext.Messages.Where(m => m.Username == username));
            await dbContext.SaveChangesAsync();
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
