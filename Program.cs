
using VideoUrlToChatBot;
using Python.Runtime;
using ChatBotCoachWebsite.Helpers;
using VideoUrlToChatBot.MigratedFromSite.Services;
using Microsoft.Extensions.Configuration;
using VideoUrlToChatBot.Models;

class Program
{
    static async Task Main(string[] args)
    {
        //set audio output path
        string path = "InputsOutputsAITraining\\AudioOutput";
        string audioOutputPath = Path.Combine(Directory.GetCurrentDirectory(), path);

        Console.WriteLine("Hello, welcome!");
        Console.WriteLine("Choose the desired action that you want to take. Enter the desired number for the action (ex: '1', '2')" +
            "\n Type 'exit' to stop and exit the program. ");

        Console.WriteLine("1. Download links in text file to .mp3");
        Console.WriteLine("2. Transcribe the .mp3 files to .txt documents");
        Console.WriteLine("3. Upsert the .txt documents to Pinecone index database");
        string selection = Console.ReadLine();
        while(selection != "exit")
        {
            if (selection == "1")
            {
                //download the links to mp3
                CMDCommand cmdCommand = new(audioOutputPath);
                cmdCommand.CMDDownloadCommand();
            }
            if (selection == "2")
            {
                //transcribe mp3 to txt files
                var transcribe = new TranscribeMp3ToTxt();
                transcribe.TranscribeMp3(audioOutputPath);
            }
            if (selection == "3")
            {
                //upsert txt documents to Pinecone index
                var dataProvider = new FileDataProvider();
                var keyProvider = new TextFileKeyProvider();
                var openAiProvider = new OpenAIService(new TextFileKeyProvider());
                var checkAsciiCompatible = new CheckAsciiCompatible();
                var buildPineconeIndex = new BuildPineconeIndex(dataProvider, keyProvider, openAiProvider, checkAsciiCompatible);
                await buildPineconeIndex.UpsertPineconeIndexAsync();
            }

            selection = Console.ReadLine();
        }


        /*
        //set login
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        var secretProvider = config.Providers.First();
        secretProvider.TryGet("YoutubeLogin:Username", out var username);
        secretProvider.TryGet("YoutubeLogin:Password", out var password);

        //set audio output path
        string path = "InputsOutputsAITraining\\AudioOutput";
        string audioOutputPath = Path.Combine(Directory.GetCurrentDirectory(), path);

        //download youtube link to mp3
        var downloadVideos = new DownloadYoutubeToMp3(audioOutputPath);
        await downloadVideos.DownloadClipToFile();
        */

    }
}
