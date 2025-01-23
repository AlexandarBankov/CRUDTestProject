using CRUDTestProject.Data.Entities;
using CRUDTestProject.Data.Repositories;

namespace CRUDTestProject.Services
{
    public class BadWordsHandler(IBadWordsRepository repository) : IBadWordsHandler
    {
        public void AddRange(IEnumerable<string> badWords)
        {
            repository.AddRange(badWords.Select(w => new BadWord() { Word = w }));
        }

        public void CheckForBadWords(IEnumerable<string> toCheck)
        {
            foreach (var badWord in repository.BadWords)
            {
                foreach (var item in toCheck)
                {
                    if (item.Contains(badWord.Word)) throw new Exception($"{badWord} is a bad word!");
                }
            }
        }

        public void Remove(string badWord)
        {
            repository.Remove(new() { Word = badWord });
        }
    }
}
