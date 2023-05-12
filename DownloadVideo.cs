
using NYoutubeDL;
using NYoutubeDL.Options;
using System.IO;

public class DownloadVideo
{
    private YoutubeDLP _youtubeDlp = new();
    private string inputUrlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "urlLinks.txt");

    //using a library for YoutubeDL program which downloads videos from most popular websites
    //this method is used for twitch clips but can accept any other videos that YoutubeDL allows
    public async Task DownloadClipToFile()
    {
        //get url links from text file
        string[] urlLinks = GetUrlLinks();

        //get current clip download options
        _youtubeDlp.Options = Options.Deserialize(File.ReadAllText("options.config"));

        //setting clip download options to config
        _youtubeDlp.YoutubeDlPath = "yt-dlp.exe";
        _youtubeDlp.Options.FilesystemOptions.WindowsFilenames = true;
        _youtubeDlp.Options.VerbositySimulationOptions.Simulate = false;
        _youtubeDlp.Options.VerbositySimulationOptions.DumpSingleJson = false;
        File.WriteAllText("options.config", _youtubeDlp.Options.Serialize());

        //download the videos
        foreach (string url in urlLinks)
        {
            try
            {
                _youtubeDlp.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
                _youtubeDlp.StandardErrorEvent += (sender, errorOutput) => Console.WriteLine(errorOutput);


                string formattedUrl = FormatUrlForFile(url);
                string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"VideoOutput\\{formattedUrl}");
                _youtubeDlp.Options.FilesystemOptions.Output = outputFilePath;
                File.WriteAllText("options.config", _youtubeDlp.Options.Serialize());

                Console.WriteLine("Starting download...");
                Console.WriteLine("Your clip is currently being downloaded");
                _youtubeDlp.Download(url);

                //if any errors occur in the download, then try updating yt-dlp program to the latest version (in hopes it will fix)
                List<string> errors = _youtubeDlp.Info.Errors;
                if (errors.Count > 0)
                {
                    //_updateYoutubeDLP.UpdateYoutubeDLP();
                    await _youtubeDlp.DownloadAsync(url);
                }

                //if mp4 is downloaded and exists, delete url from the input Url file
                bool doesFileExist = DoesFileExist(outputFilePath, formattedUrl);
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
                _youtubeDlp = new YoutubeDLP();
                return;
            }
        }

    }

    public string[] GetUrlLinks()
    {
        var text = File.ReadAllText(inputUrlFilePath);
        var urlLinks = text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        return urlLinks;
    }

    public static string FormatUrlForFile(string input)
    {
        if (input == null)
        {
            return null;
        }
        return input.Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "");
    }

    public bool DoesFileExist(string filePath, string fileName)
    {
        bool doesFileExist = false;
        filePath = Path.Combine(Directory.GetCurrentDirectory(), $"VideoOutput");
        var files = Directory.EnumerateFiles(filePath);
        doesFileExist = files.Any(file => Path.GetFileNameWithoutExtension(file) == fileName);
        if (File.Exists(filePath) == true)
            doesFileExist = true;
        return doesFileExist;
    }

    public void DeleteUrlFromFile(string url)
    {
        var lines = File.ReadAllLines(inputUrlFilePath);
        var newLines = lines.Where(line => line != url).ToArray();
        File.WriteAllLines(inputUrlFilePath, newLines);
    }

}
