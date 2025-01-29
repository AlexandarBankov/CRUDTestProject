using CRUDTestProject.Data.Repositories;

namespace CRUDTestProject.Services.Implementation
{
    public class BackupHandler(IBackupRepository repository) : IBackupHandler
    {
        public async Task Create()
        {
            repository.Create(DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss"));
        }

        public async Task<IEnumerable<string>> GetNames()
        {
            return repository.GetNames();
        }
    }
}
