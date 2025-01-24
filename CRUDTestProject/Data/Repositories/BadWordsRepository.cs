using CRUDTestProject.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUDTestProject.Data.Repositories
{
    public class BadWordsRepository(ApplicationDbContext dbContext) : IBadWordsRepository
    {
        public IEnumerable<BadWord> BadWords => dbContext.BadWords.AsNoTracking();

        public void AddRange(IEnumerable<BadWord> badWords)
        {
            dbContext.BadWords.AddRange(badWords);
            dbContext.SaveChanges();
        }

        public void Remove(BadWord badWord)
        {
            dbContext.BadWords.Remove(badWord);
            dbContext.SaveChanges();
        }
    }
}
