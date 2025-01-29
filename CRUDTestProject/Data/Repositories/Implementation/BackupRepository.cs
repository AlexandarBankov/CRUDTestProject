using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace CRUDTestProject.Data.Repositories.Implementation
{
    public class BackupRepository(ApplicationDbContext dbContext, IConfiguration configuration) : IBackupRepository
    {
        public void Create(string name)
        {
            var temp = Directory.CreateDirectory(Path.Combine(configuration["BackupPath"], $"{Guid.NewGuid()}"));
            try
            {
                dbContext.Database.ExecuteSqlRaw($"BACKUP DATABASE [MessagesDb] TO  DISK = N'{Path.Combine(temp.FullName, "backup.bak")}'");

                var folderPath = Path.Combine(configuration["BackupPath"], name);
                Directory.CreateDirectory(folderPath);

                ZipFile.CreateFromDirectory(temp.FullName, Path.Combine(folderPath, name + ".zip"));
            }
            finally 
            {
                temp.Delete(true);
            }
        }

        public IEnumerable<string> GetNames()
        {
            return Directory.GetDirectories(configuration["BackupPath"]);
        }
    }
}
