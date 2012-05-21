using System.IO;
using System.Text;

namespace Services
{
    public class FileStorageService : IStorageService
    {
        public char DirectorySeparatorChar
        {
            get { return Path.DirectorySeparatorChar; }
        }

        public string ReadAllText(string path, Encoding encoding)
        {
            return File.ReadAllText(path, encoding);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public string GetParentDirectoryFullName(string filePath)
        {
            var directoryName = GetDirectoryName(filePath);

            if (string.IsNullOrEmpty(directoryName))
                return null;

            var directoryInfo = new DirectoryInfo(directoryName);

            return directoryInfo.Parent != null
                       ? directoryInfo.Parent.FullName
                       : directoryInfo.Root.FullName;
        }

        public void WriteAllText(string destinationFilePath, string content, Encoding encoding)
        {
            File.WriteAllText(destinationFilePath, content, encoding);
        }

        public string GetFileNameWithoutExtension(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        public string GetExtension(string filePath)
        {
            return Path.GetExtension(filePath);
        }

        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }
    }
}