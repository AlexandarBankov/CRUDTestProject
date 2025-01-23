using CRUDTestProject.Data.Entities;

namespace CRUDTestProject.Data.Repositories
{
    public interface IBadWordsRepository
    {
        IEnumerable<BadWord> BadWords { get; }
        void AddRange(IEnumerable<BadWord> badWords);
        void Remove(BadWord badWord);
    }
}
