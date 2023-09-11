using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoUrlToChatBot
{
    public class TranscribeMp3ToTxt
    {

        public void TranscribeMp3(string mp3Path)
        {
            var pythonDllLocation = "C:\\Python310\\python310.dll"; 
            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDllLocation);

            PythonEngine.Initialize();

            var mp3InputPath = mp3Path;
            var files = Directory.GetFiles(mp3InputPath, "*.mp3");
            var transcribeOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "InputsOutputsAITraining\\docs");
            string modelType = "small";
            using (Py.GIL())
            {
                var whisper = Py.Import("whisper");
                var model = whisper.InvokeMethod("load_model", new PyString(modelType));
                foreach (var file in files)
                {
                    if (!File.Exists(file))
                    {
                        var result = model.InvokeMethod("transcribe", new PyString(file));
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        File.WriteAllText(transcribeOutputDirectory + $"\\{fileName}.txt", result["text"].As<string>());
                    }
                    else
                    {
                        Console.WriteLine($"This file already exists: {file}. Do not transcribe");
                    }
                }
            }
        }
    }
}
