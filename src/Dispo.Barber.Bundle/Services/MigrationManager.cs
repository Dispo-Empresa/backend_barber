using System.Text.RegularExpressions;
using Dispo.Barber.Bundle.Entities;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Bundle.Services
{
    public class MigrationFile
    {
        public string Version { get; set; }
        public string Path { get; set; }
    }

    public class MigrationManager : IMigrationManager
    {
        private readonly ApplicationContext context;

        public MigrationManager(ApplicationContext context)
        {
            this.context = context;
        }

        public void Migrate()
        {
            SetupMigrationsTable();
            var currentVersion = LastAppliedMigration();
            var migrations = GetMigrations(currentVersion).ToList();
            foreach (var migrationFile in migrations)
            {
                ApplyMigration(migrationFile);
            }
        }

        public IEnumerable<MigrationFile> GetMigrations(string currentVersion)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var migrationsDirectory = Path.Combine(currentDirectory, "..", "Dispo.Barber.Bundle/Migrations");
            var migrationFiles = Directory.GetFiles(migrationsDirectory, "*.pgsql");
            foreach (var file in migrationFiles)
            {
                var cleanedPath = Path.GetFullPath(file);
                var fileName = Path.GetFileName(cleanedPath);
                var fileVersion = ExtractNumberFromFileName(fileName);
                if (!ShouldApplyMigration(currentVersion, fileVersion))
                {
                    continue;
                }
                yield return new MigrationFile { Path = file, Version = fileVersion };
            }
        }

        private string GetVersion(Migration? migration)
        {
            if (migration == null)
            {
                return "000";
            }

            return migration.Version;
        }

        private string ExtractNumberFromFileName(string fileName)
        {
            var regex = new Regex(@"^\d+");
            var match = regex.Match(fileName);
            return match.Success ? match.Value : string.Empty;
        }

        private bool ShouldApplyMigration(string currentVersion, string fileVersion)
        {
            return Convert.ToInt64(currentVersion) < Convert.ToInt64(fileVersion);
        }

        private void SetupMigrationsTable()
        {
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ""Migrations"" (
                    ""DatabaseVersion"" VARCHAR NOT NULL,
                    ""UpdatedAt"" TIMESTAMP NOT NULL
                );
            ");
        }

        private void UpdateVersion(string version)
        {
            context.Database.ExecuteSqlRaw(@"INSERT INTO ""Migrations"" (""DatabaseVersion"", ""UpdatedAt"") VALUES (@version, @updatedAt);",
                new Npgsql.NpgsqlParameter("@version", version),
                new Npgsql.NpgsqlParameter("@updatedAt", DateTime.UtcNow));
        }

        private string LastAppliedMigration()
        {
            FormattableString query = $"SELECT * FROM \"Migrations\"";
            var migration = context.Database.SqlQuery<Migration>(query).OrderBy(o => o.UpdatedAt).LastOrDefault();
            return GetVersion(migration);
        }

        private void ApplyMigration(MigrationFile migrationFile)
        {
            var sql = File.ReadAllText(migrationFile.Path);
            context.Database.ExecuteSqlRaw(sql);
            UpdateVersion(migrationFile.Version);
        }
    }
}
