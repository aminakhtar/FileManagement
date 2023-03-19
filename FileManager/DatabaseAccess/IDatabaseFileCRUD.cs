using FileManager.Models;

namespace FileManager.DatabaseAccess
{
    public interface IDatabaseFileCRUD
    {
        public bool IsFileExist(string fileName);

        public InputFile? GetLatestVersionFile(string fileName);
        public bool InsertFile(InputFile file);
        public bool DeleteFile(string file, int verion = 0);
        public List<InputFile> ListFiles();
        public List<InputFile> ListAllVersionsOfAFile(string fileName);

        public List<InputFile> ListAllFilesAndVersions();
    }
}
