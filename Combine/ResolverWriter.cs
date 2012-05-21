namespace Combine
{
    using System.Text;

    using Services;

    public class ResolverWriter : IResolverWriter
    {
        private readonly IResolver _resolver;
        private readonly IStorageService _storageService;

        public Encoding Encoding { get; set; }

        public ResolverWriter(IStorageService storageService, IResolver resolver)
        {
            this._storageService = storageService;
            this._resolver = resolver;
            this.Encoding = Encoding.ASCII;
        }
        
        public void ResolveTo(string sourceFilePath, string destinationFilePath)
        {
            this._storageService.WriteAllText(destinationFilePath, this._resolver.Resolve(sourceFilePath), this.Encoding);
        }

        public void Resolve(string sourceFilePath)
        {
            this.Resolve(sourceFilePath, null);
        }

        public void Resolve(string sourceFilePath, string postfix)
        {
            var values = string.IsNullOrEmpty(postfix) ? string.Empty : "." + postfix;

            var filename = this._storageService.GetFileNameWithoutExtension(sourceFilePath);
            var extension = this._storageService.GetExtension(sourceFilePath);
            var path = this._storageService.GetDirectoryName(sourceFilePath);

            if (!string.IsNullOrEmpty(path) && !path.EndsWith(this._storageService.DirectorySeparatorChar.ToString()))
                path += this._storageService.DirectorySeparatorChar;

            var destinationFilePath = string.Concat(path, filename, values, extension);

            this.ResolveTo(sourceFilePath, destinationFilePath);
        }
    }
}