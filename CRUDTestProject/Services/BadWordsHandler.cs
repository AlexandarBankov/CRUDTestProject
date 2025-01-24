using CRUDTestProject.Data.Entities;
using CRUDTestProject.Data.Repositories;
using CRUDTestProject.Middleware.Exceptions;

namespace CRUDTestProject.Services
{
    public class BadWordsHandler(IBadWordsRepository repository) : IBadWordsHandler
    {
        public void AddRange(IEnumerable<string> badWords)
        {
            var toAdd = badWords.Where(w => repository.BadWords.FirstOrDefault(bw => bw.Word == w) is null);
            repository.AddRange(toAdd.Select(w => new BadWord() { Word = w })); 
        }

        public void CheckForBadWords(IEnumerable<string> toCheck)
        {
            foreach (var badWord in repository.BadWords)
            {
                foreach (var item in toCheck)
                {
                    if (item.Contains(badWord.Word)) throw new BadWordException($"{badWord.Word} is a bad word!");
                }
            }
        }

        public void Remove(string badWord)
        {
            var toRemove = repository.BadWords.FirstOrDefault(w => w.Word == badWord) ?? throw new NotFoundException("Bad word for removal not found.");
            repository.Remove(toRemove);
        }
    }
}
