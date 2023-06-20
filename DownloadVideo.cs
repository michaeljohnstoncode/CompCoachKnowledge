using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;

public class DownloadVideo
{
    private YoutubeDL _youtubeDl = new();
    private string _inputUrlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "InputsOutputsAITraining\\urlLinks.txt");
    private string _audioOutputPath;

    public DownloadVideo(string audioOutputPath)
    {
        _audioOutputPath = audioOutputPath;
    }

    //using a library for YoutubeDL program which downloads videos from most popular websites
    public async Task DownloadClipToFile()
    {
        //get url links from text file
        string[] urlLinks = GetUrlLinks();

        //setting clip download options to config
        _youtubeDl.YoutubeDLPath = "yt-dlp.exe";
        _youtubeDl.FFmpegPath = "ffmpeg.exe";

        //download the videos
        foreach (string url in urlLinks)
        {
            try
            {
                //output error logs to console
                var ytdlProc = new YoutubeDLProcess();
                //capture the standard output and error output
                ytdlProc.OutputReceived += (o, e) => Console.WriteLine(e.Data);
                ytdlProc.ErrorReceived += (o, e) => Console.WriteLine("ERROR: " + e.Data);

                //set output folder for download
                _youtubeDl.OutputFolder = _audioOutputPath;

                //download the video as mp3
                Console.WriteLine("Starting download...");
                Console.WriteLine("Your clip is currently being downloaded");
                var mp3 = await _youtubeDl.RunAudioDownload(url, AudioConversionFormat.Mp3);
                string oldFileName = mp3.Data;
                Console.WriteLine($"Path that ytdlp wrote to: {oldFileName}");


                //get title and uploader name to create file name
                var res = await _youtubeDl.RunVideoDataFetch(url);
                VideoData video = res.Data;
                string title = video.Title;
                string uploader = video.Uploader;
                string formatFileName = title + " uploader " + uploader;
                string fileName = FormatNameForFile(formatFileName);
                string newFileName = Path.Combine(_audioOutputPath, $"{fileName}.mp3");

                //change file name by using File.Move
                RenameFile(oldFileName, newFileName);

                //if mp4 is downloaded and exists, delete url from the input Url file
                bool doesFileExist = DoesFileExist(fileName);
                if (doesFileExist == true)
                {
                    DeleteUrlFromFile(url);
                    Console.WriteLine($"Deleted Url: {url} from file after download complete.");
                }

            }
            //task canceled exception for catching when canceling download (I don't know why this happens). creates new downloader instance
            catch (System.Threading.Tasks.TaskCanceledException ex)
            {
                Console.WriteLine($"exception: {ex}");
                Console.WriteLine("Download has canceled");
                _youtubeDl = new YoutubeDL();
                return;
            }
        }

    }

    public void RenameFile(string oldFilePath, string newFilePath)
    {
        // Check if the old file exists
        if (File.Exists(oldFilePath))
        {
            // Check if the new file already exists
            if (!File.Exists(newFilePath))
            {
                File.Move(oldFilePath, newFilePath);
            }
            else
            {
                Console.WriteLine("A file with the new name already exists.");
            }
        }
        else
        {
            Console.WriteLine("The file to be renamed does not exist.");
        }
    }

    public string[] GetUrlLinks()
    {
        var text = File.ReadAllText(_inputUrlFilePath);
        var urlLinks = text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        return urlLinks;
    }

    public static string FormatNameForFile(string input)
    {
        if (input == null)
        {
            return null;
        }
        return input.Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("|", "")
                    .Replace("*", "").Replace("<", "").Replace(">", "").Replace("`", "").Replace("\"", "");
    }

    public bool DoesFileExist(string fileName)
    {
        bool doesFileExist = false;
        string filePath = _audioOutputPath;
        var files = Directory.EnumerateFiles(filePath);
        doesFileExist = files.Any(file => Path.GetFileNameWithoutExtension(file) == fileName);
        if (File.Exists(filePath) == true)
            doesFileExist = true;
        return doesFileExist;
    }

    public void DeleteUrlFromFile(string url)
    {
        var lines = File.ReadAllLines(_inputUrlFilePath);
        var newLines = lines.Where(line => line != url).ToArray();
        File.WriteAllLines(_inputUrlFilePath, newLines);
    }

}
