namespace CRUDTestProject.Services
{
    public interface IBackupHandler
    {
        Task Create();

        Task<IEnumerable<string>> GetNames();
    }
}
