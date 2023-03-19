using FileManager.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq;
using System.Reflection.Metadata;
using InputFile = FileManager.Models.InputFile;

namespace FileManager.DatabaseAccess
{
    public class DatabaseFileCRUD : IDatabaseFileCRUD
    {
        FileContext db = new FileContext();
        public bool IsFileExist(string fileName)
        {
            return db.InputFiles.Any(x => x.FileName == fileName);
        }

        public bool InsertFile(InputFile file)
        {
            db.Add(file);
            return db.SaveChanges() > 0;
        }
        public bool DeleteFile(string file, int version = 0)
        {
            // If the version is not provided, delete the latest version
            InputFile? inputFile = (version != 0) ?
                GetSpecificVersionFile(file, version) : 
                GetLatestVersionFile(file);

            if(inputFile != null)
                db.Remove(inputFile);

            return db.SaveChanges() > 0;
        }
        
        public List<InputFile> ListAllVersionsOfAFile(string fileName)
        {
            // List all files with different versions and the same name
            return db.InputFiles
                    .Where(x => x.FileName == fileName)
                    .ToList();
        }

        public List<InputFile> ListFiles()
        {
            // List all files regardless of their version
            return db.InputFiles
                .GroupBy(p => p.FileName)
                .Select(g => g.First())
                .ToList();
        }

        public InputFile? GetLatestVersionFile(string fileName)
        {
            // Return the latest version of the file
            return db.InputFiles
                .Where(x => x.FileName == fileName)
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();
        }
        public InputFile? GetSpecificVersionFile(string fileName, int version)
        {
            // Get a specific version of the file
            return db.InputFiles
                .Where(x => x.FileName == fileName && x.Version == version)
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();
        }

        public List<InputFile> ListAllFilesAndVersions()
        {
            return db.InputFiles.ToList();
        }
    }
}
