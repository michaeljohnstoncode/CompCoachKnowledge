namespace VideoUrlToChatBot.MigratedFromSite.Services
{
    public interface ICustomDataProvider
    {
        Dictionary<string, string> ReadCustomData();
    }

    public class TextFileCustomDataProvider : ICustomDataProvider
    {
       // private const string _customDataDir = @"C:\\Users\\Michael\\Desktop\\docs";
        private string _customDataDir = Path.Combine(Directory.GetCurrentDirectory(), "InputsOutputsAITraining\\docs");

        public Dictionary<string, string> ReadCustomData()
        {
            //check if directory exists
            if (!Directory.Exists(_customDataDir))
            {
                Console.WriteLine($"Directory {_customDataDir} does not exist.");
                return null;
            }

            //get all .txt files in directory
            string[] files = Directory.GetFiles(_customDataDir, "*.txt");

            //loop through and read all txt files
            Dictionary<string, string> customData = new();
            foreach (string file in files)
            {
                string fileData = File.ReadAllText(file);
                string fileName = Path.GetFileName(file);
                customData.Add(fileName, fileData);
            }

            return customData;
        }
    }

}
