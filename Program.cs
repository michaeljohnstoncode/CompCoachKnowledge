
using VideoUrlToChatBot;
using Python.Runtime;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        
        string videoOutputPath = Path.Combine(Directory.GetCurrentDirectory(), "InputsOutputsAITraining\\VideoOutput");
        string audioOutputPath = Path.Combine(Directory.GetCurrentDirectory(), "InputsOutputsAITraining\\AudioOutput");

        var downloadVideos = new DownloadVideo(audioOutputPath);
        await downloadVideos.DownloadClipToFile();

        /*
        var convertToAudio = new ConvertVideoToMp3();
        string[] filesToConvert = Directory.GetFiles(videoOutputPath);
        convertToAudio.ConvertToMp3(filesToConvert, audioOutputPath);
        */

        var transcribe = new TranscribeMp3ToTxt();
        transcribe.TranscribeMp3(audioOutputPath);
        
        /*
        Console.WriteLine($"Do you want to run the AI trainer? This costs money [Y/N]");
        string response = Console.ReadLine();
        if(response == "Y" || response == "y" || response == "yes")
        {
            var trainAi = new TrainAI();
            Console.WriteLine("AI trainer will run");
            trainAi.RunAITrainer();
        }
        else
        {
            Console.WriteLine("AI trainer will NOT run");
        }
        */

        // TODO: Now, call app.py to train the compcoach AI. Ask for consent in console to train AI [Y/N] because it costs money.
        // TODO: When AI is trained, allow to ask questions through console and receive response in console from the AI ip address
                
    }
}
