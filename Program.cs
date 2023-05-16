
using Python.Runtime;
using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var downloadVideos = new DownloadVideo();

        // downloadVideos.DownloadClipToFile();

        // 5/12/23 note: I finished the conversion of input URL link to output video file.
        // 5/15/23 note: finished whisper conversion.
        // TODO: Now, call app.py to train the compcoach AI. Ask for consent in console to train AI [Y/N] because it costs money.
        
        // TODO: When AI is trained, allow to ask questions through console and receive response in console from the AI ip address

        var pythonDllLocation = "C:\\Python310\\python310.dll";
        Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDllLocation);

        PythonEngine.Initialize();

        var mp3InputDirectory = "C:\\Users\\Michael\\source\\AIBotDocs\\mp3 audios";
        var files = Directory.GetFiles(mp3InputDirectory, "*.mp3");
        var transcribeOutputDirectory = "C:\\Users\\Michael\\source\\AIBotDocs\\docs";
        string modelType = "tiny";
        using (Py.GIL())
        {
            var whisper = Py.Import("whisper");
            var model = whisper.InvokeMethod("load_model", new PyString(modelType));
            foreach(var file in files)
            {
                var result = model.InvokeMethod("transcribe", new PyString(file));
                var fileName = Path.GetFileNameWithoutExtension(file);
                File.WriteAllText(transcribeOutputDirectory + $"\\{fileName}.txt", result["text"].As<string>());
            }

        }
    }
}
