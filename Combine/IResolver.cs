namespace Combine
{
    using System.Text;

    public interface IResolver
    {
        Encoding Encoding { get; set; }
        string Resolve(string filePath);
    }
}