
using VideoUrlToChatBot;
using Python.Runtime;
using ChatBotCoachWebsite.Helpers;
using VideoUrlToChatBot.MigratedFromSite.Services;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        string audioOutputPath = Path.Combine(Directory.GetCurrentDirectory(), "InputsOutputsAITraining\\AudioOutput");

        //download youtube link to mp3
        var downloadVideos = new DownloadYoutubeToMp3(audioOutputPath);
        await downloadVideos.DownloadClipToFile();
        Console.WriteLine("All links have been converted to mp3's. Would you like to transcribe these now? (y/n)");
        string confirmTranscribe = Console.ReadLine();
        if(confirmTranscribe != "y") 
        {
            return;
        }

        //transcribe mp3 to txt files
        var transcribe = new TranscribeMp3ToTxt();
        transcribe.TranscribeMp3(audioOutputPath);
        Console.WriteLine("All mp3's have been transcribed. Would you like to upsert to Pinecone database index? (y/n)");
        string confirmUpsert = Console.ReadLine();
        if (confirmUpsert != "y")
        {
            return;
        }

        //upsert txt file data to Pinecone index
        var dataProvider = new TextFileCustomDataProvider();
        var keyProvider = new TextFileKeyProvider();
        var openAiProvider = new OpenAIService(new TextFileKeyProvider());
        var buildPineconeIndex = new BuildPineconeIndex(dataProvider, keyProvider, openAiProvider);
        await buildPineconeIndex.UpsertPineconeIndexAsync();
    }
}
