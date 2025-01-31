namespace CRUDTestProject.Data.Repositories
{
    public interface IBackupRepository
    {
        void Create(string name);
        IEnumerable<string> GetNames();
    }
}
