using System.IO;
using CopyToLocales.Postgressql.Dto;
using Microsoft.EntityFrameworkCore;

namespace CopyToLocales.Postgressql
{
    public class ApplicationContext : DbContext
    {
        public DbSet<LocalizationStringModel> LocalizationStringModels { get; set; }
        private string DbPath;
        
        public ApplicationContext()
        {
            DbPath = Path.Combine("Z:\\Repositories\\GitHub\\CopyResx\\CopyToLocales\\bin", "test.db");
                
            Database.EnsureCreated();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
            // optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=usersdb;Username=postgres;Password=здесь_указывается_пароль_от_postgres");
        }
    }
}