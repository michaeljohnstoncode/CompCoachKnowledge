namespace VideoUrlToChatBot.MigratedFromSite.Services
{
    public interface IKeyProvider
    {
        string GetKey(string keyName);
    }

    public class TextFileKeyProvider : IKeyProvider
    {
        //current keys: "openaikey", "pineconekey"
        private const string _apiKeysDir = @"C:\\Users\\Michael\\Desktop\\apikeys";

        public string GetKey(string keyName)
        {
            var filePath = Path.Combine(_apiKeysDir, $"{keyName}.txt");
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath).Trim();
            }

            throw new ArgumentException($"Key {keyName} not found.");
        }
    }
    /*
        //KeyService class currently not necessary because only will need the TextFileKeyProvider class. I will keep this because I am still learning to integrate SOLID principles
        public class KeyService
        {
            private readonly IKeyProvider keyProvider;

            public KeyService(IKeyProvider keyProvider)
            {
                this.keyProvider = keyProvider;
            }

            public string GetApiKey(string keyName)
            {
                return keyProvider.GetKey(keyName);
            }
        }
    */
}
