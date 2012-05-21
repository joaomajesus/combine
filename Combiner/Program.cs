namespace Combine
{
    using Combine.Properties;

    using Services;

    public class Program
    {
        public static void Main(string[] args)
        {
            var fileStorageService = new FileStorageService();

            if (args.Length == 2)
                LocalSelectableFileSettingsProvider.ExeConfigFilename = args[1];
            
            var resolverWriter = new ResolverWriter(fileStorageService, new Resolver(fileStorageService));
            
            resolverWriter.Resolve(args[0], Settings.Default.defaultPostfix);
        }
    }
}
