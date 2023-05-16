
using CompCoachKnowledge;
using Python.Runtime;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        string videoOutputPath = Path.Combine(Directory.GetCurrentDirectory(), "InputsOutputsAITraining\\VideoOutput");
        string audioOutputPath = Path.Combine(Directory.GetCurrentDirectory(), "InputsOutputsAITraining\\AudioOutput");

        var downloadVideos = new DownloadVideo(videoOutputPath);
        downloadVideos.DownloadClipToFile();

        var convertToAudio = new ConvertVideoToMp3();
        string[] filesToConvert = Directory.GetFiles(videoOutputPath);
        convertToAudio.ConvertToMp3(filesToConvert, audioOutputPath);

        var transcribe = new TranscribeMp3ToTxt();
        transcribe.TranscribeMp3(audioOutputPath);

        // TODO: Now, call app.py to train the compcoach AI. Ask for consent in console to train AI [Y/N] because it costs money.
        // TODO: When AI is trained, allow to ask questions through console and receive response in console from the AI ip address
                
    }
}
