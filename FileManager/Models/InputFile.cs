using Microsoft.EntityFrameworkCore;

namespace FileManager.Models
{
    public class InputFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string? ContentType { get; set; }
        public long Length { get; set; }
        public int Version { get; set; }
    }
}
