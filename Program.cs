
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

        //transcribe mp3 to txt files
        var transcribe = new TranscribeMp3ToTxt();
        transcribe.TranscribeMp3(audioOutputPath);

        //upsert txt file data to Pinecone index
        var dataProvider = new TextFileCustomDataProvider();
        var keyProvider = new TextFileKeyProvider();
        var openAiProvider = new OpenAIService(new TextFileKeyProvider());
        var buildPineconeIndex = new BuildPineconeIndex(dataProvider, keyProvider, openAiProvider);
        await buildPineconeIndex.UpsertPineconeIndexAsync();
    }
}
