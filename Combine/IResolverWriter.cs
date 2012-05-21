namespace Combine
{
    using System.Text;

    public interface IResolverWriter
    {
        Encoding Encoding { get; set; }
        void ResolveTo(string sourceFilePath, string destinationFilePath);
        void Resolve(string sourceFilePath);
        void Resolve(string sourceFilePath, string postfix);
    }
}