using System.Text;

namespace Services
{
    public interface IStorageService
    {
        char DirectorySeparatorChar { get; }
        string ReadAllText(string path, Encoding encoding);
        bool Exists(string path);
        string GetFileName(string path);
        string GetDirectoryName(string path);
        string GetParentDirectoryFullName(string filePath);
        void WriteAllText(string destinationFilePath, string content, Encoding encoding);
        string GetFileNameWithoutExtension(string filePath);
        string GetExtension(string filePath);
        string GetFullPath(string path);
    }
}