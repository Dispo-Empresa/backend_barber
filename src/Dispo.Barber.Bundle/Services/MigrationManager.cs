using Dispo.Barber.Bundle.Services;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Bundle.Entities
{
    public class MigrationManager : IMigrationManager
    {
        private readonly ApplicationContext context;

        public MigrationManager(ApplicationContext context)
        {
            this.context = context;
        }

        public void Migrate()
        {
            FormattableString query = $"SELECT * FROM \"Migrations\"";
            var migration = context.Database.SqlQuery<Migration>(query).First();
            var migrations = GetMigrations();
        }

        public IEnumerable<string> GetMigrations()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string migrationsDirectory = Path.Combine(currentDirectory, "..", "Migrations");
            string[] migrationFiles = Directory.GetFiles(migrationsDirectory, "*.pgsql");
            foreach (string file in migrationFiles)
            {
                yield return file;
            }
        }
    }
}
