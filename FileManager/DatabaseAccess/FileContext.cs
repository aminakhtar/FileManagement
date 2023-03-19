using FileManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace FileManager.DatabaseAccess
{
    public class FileContext : DbContext
    {
        public DbSet<InputFile> InputFiles { get; set; }
        public string DbPath { get; }
        public FileContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);

            DbPath = Path.Join(path, "filemanagement.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
    
}
