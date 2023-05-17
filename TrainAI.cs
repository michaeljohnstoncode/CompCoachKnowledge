using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoUrlToChatBot
{
    public class TrainAI
    {

        public void RunAITrainer()
        {
            string python = @"C:\Python310\python.exe";
            string script = Path.Combine(Directory.GetCurrentDirectory(), "InputsOutputsAITraining\\app.py");
            ProcessStartInfo startInfo = new ProcessStartInfo(python)
            {
                Arguments = script,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false,
                WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(script)) // Set the working directory to the directory of the script
            };

            Process process = new Process() { StartInfo = startInfo };
            process.Start();

            string result = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            Console.WriteLine(result);
        }
    }
}
