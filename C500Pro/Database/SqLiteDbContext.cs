using System.Data.Entity;
using System.Data.SQLite;

namespace SQLiteDB
{
    public class SqLiteDbContext<T> : DbContext where T : DbContext
    {
        private readonly int _currentSchemaVersion;
        public SqLiteDbContext(string connectionString, int currentSchemaVersion)
            : base(connectionString)
        {
            _currentSchemaVersion = currentSchemaVersion;
        }

        public SqLiteDbContext(SQLiteConnection connection, int currentSchemaVersion)
            : base(connection, false)
        {
            _currentSchemaVersion = currentSchemaVersion;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new CreateOrMigrateDatabaseInitializer<T>(
                                        modelBuilder, _currentSchemaVersion));
        }
    }
}
