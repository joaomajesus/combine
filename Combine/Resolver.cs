namespace Combine
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;

    using Combine.Properties;

    using Services;

    public class Resolver : IResolver
    {
        private static readonly Regex ImportRegex = new Regex(Settings.Default.ImportRegexString, RegexOptions.IgnoreCase);

        private readonly HashSet<string> addedFiles = new HashSet<string>();

        private readonly IStorageService storageService;
        private string baseDirectory;
       
        public Resolver(IStorageService storageService)
        {
            this.storageService = storageService;
            this.Encoding = Encoding.ASCII;
        }

        public Encoding Encoding { get; set; }
        
        public string Resolve(string filePath)
        {
            var header = string.Format(Settings.Default.FileHeaderFormat, filePath);

            var fileText = header + this.storageService.ReadAllText(filePath, this.Encoding);

            this.addedFiles.Clear();

            this.baseDirectory = this.storageService.GetParentDirectoryFullName(filePath);

            return this.ResolveRecursive(filePath, fileText);
        }

        protected virtual string ResolveRecursive(string filePath, string fileText)
        {
            Trace.Indent();
            Trace.TraceInformation("Resolving file: " + filePath);

            var ret = ImportRegex.Replace(fileText,
                                          match => this.Evaluate(filePath, match));

            Trace.Unindent();

            return ret;
        }

        private string Evaluate(string filePath, Match match)
        {
            var fullPath = this.GetFullPath(filePath,
                                       match.Groups["path"].Value,
                                       match.Groups["absolute"].Length > 0);

            Trace.Indent();

            if (!this.storageService.Exists(fullPath))
            {
                Trace.TraceInformation("Could not find file: " + fullPath);
                Trace.Unindent();

                //throw new ApplicationException("Could not find file: " + fullPath);
                return string.Empty;
            }

            if (!this.addedFiles.Add(fullPath))
            {
                Trace.TraceInformation("File already inlined: " + fullPath);
                Trace.Unindent();
                return string.Empty;
            }

            Trace.TraceInformation("Inlining file: " + fullPath);

            var ret1 = this.ResolveRecursive(fullPath,
                                             string.Format(Settings.Default.InlineFormat,
                                                           this.storageService.GetFileName(fullPath),
                                                           this.storageService.ReadAllText(fullPath, this.Encoding)));

            Trace.Unindent();

            return ret1;
        }

        protected virtual string GetFullPath(string filePath, string importPath, bool isImportPathAbsolute)
        {
            var matchFilePath = importPath.Replace('/', this.storageService.DirectorySeparatorChar);

            var directoryName = isImportPathAbsolute
                              ? this.baseDirectory
                              : this.storageService.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(directoryName) && !directoryName.EndsWith(this.storageService.DirectorySeparatorChar.ToString()))
                directoryName += this.storageService.DirectorySeparatorChar;
            
            return directoryName + matchFilePath;
        }
    }
}
