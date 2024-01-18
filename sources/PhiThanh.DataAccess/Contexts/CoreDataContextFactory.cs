using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PhiThanh.Core;

namespace PhiThanh.DataAccess.Contexts
{
    public class CoreDataContextFactory : IDesignTimeDbContextFactory<CoreDataContext>
    {
        public CoreDataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CoreDataContext>();

            var serverVersion = new MySqlServerVersion(new Version(AppSettings.Instance.Persistence.Major,
                    AppSettings.Instance.Persistence.Minor,
                    AppSettings.Instance.Persistence.Build));
            optionsBuilder.UseMySql(AppSettings.Instance.Persistence.CoreDataContext, serverVersion)
                .EnableSensitiveDataLogging();

            return new CoreDataContext(optionsBuilder.Options);
        }
    }
}
