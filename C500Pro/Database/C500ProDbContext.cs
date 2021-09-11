using C500Pro.Models;
using SQLite.CodeFirst;
using SQLiteDB;
using System;
using System.Data.Entity;
using System.Data.SQLite;

namespace C500Pro.Database
{
    class C500ProDbContext : DbContext
    {
        private const int CurrentSchemaVersion = 1;
        public C500ProDbContext() : base(new SQLiteConnection("data source=.\\c500pro.sqlite"), false)//, CurrentSchemaVersion) 
        {
            // Migration();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {
                var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<C500ProDbContext>(modelBuilder);
                System.Data.Entity.Database.SetInitializer(sqliteConnectionInitializer);
            }
            catch { }
        }

        public virtual DbSet<LogNetWork> LogNetWorks { get; set; }

        public void Migration()
        {
            //TryRunQuery($"CREATE TABLE DataSourceSearchContents ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, Url nvarchar(255) not null, Type nvarchar(50) null);");
            // TryRunQuery($"ALTER TABLE LogNetWorks ADD CreatedAt DATETIME NOT NULL DEFAULT (datetime(current_timestamp));");
        }

        bool TryRunQuery(string query)
        {
            try
            {
                var temp = Database.ExecuteSqlCommand(query);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
