using System.Data.Entity;
using System.IO;
using System.Linq;
using SQLite.CodeFirst;

namespace SQLiteDB
{
    public class CreateOrMigrateDatabaseInitializer<TContext> :
        SqliteCreateDatabaseIfNotExists<TContext> where TContext : DbContext
    {
        private readonly int _currentSchemaVersion;
        public CreateOrMigrateDatabaseInitializer(DbModelBuilder modelBuilder,
            int currentSchemaVersion) :
            base(modelBuilder)
        {
            _currentSchemaVersion = currentSchemaVersion;
        }
        protected override void Seed(TContext context)
        {
            base.Seed(context);
            context.Database.ExecuteSqlCommand(
                $"PRAGMA user_version={_currentSchemaVersion}");
        }
        public override void InitializeDatabase(TContext context)
        {
            base.InitializeDatabase(context);
            if (context.Database.Exists())
                Migrate(context);
        }
        private static void Migrate(DbContext context)
        {
            var currentDatabaseVersion =
                context.Database.SqlQuery<int>("PRAGMA user_version").First();
            if (!Directory.Exists("migrations")) return;
            var scriptFiles = Directory.GetFiles("migrations/", "*.sql");
            foreach (var scriptFile in scriptFiles)
                MigrateWithScriptFileFromVersion(context, scriptFile,
                    currentDatabaseVersion);
        }
        private static void MigrateWithScriptFileFromVersion(DbContext context,
            string scriptFile, int currentVersion)
        {
            var filenamePrefix = Path.GetFileName(scriptFile)?.Split('.').First();
            if (!int.TryParse(filenamePrefix, out var targetVersion) ||
                targetVersion <= currentVersion)
                return;
            var migrationScript = File.ReadAllText(scriptFile);
            context.Database.ExecuteSqlCommand(migrationScript);
        }
    }
}