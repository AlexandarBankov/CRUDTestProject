namespace CRUDTestProject.Services
{
    public interface IBadWordsHandler
    {
        //throws if there is a bad word
        void CheckForBadWords(IEnumerable<string> toCheck);
        void AddRange(IEnumerable<string> badWords);
        void Remove(string badWord);
    }
}
