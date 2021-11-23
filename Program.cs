using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace UpdatersWorld
{
    class Program
    {
        private static string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"config.txt");
            foreach (var line in lines)
            {
                if (line.Contains("VERSION_URL="))
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(line.Replace("VERSION_URL=", ""), "tempversion.txt");

                    string versionLine = File.ReadAllText("tempversion.txt");
                    if (!File.Exists("version.txt"))
                    {
                        FileInfo versionInfo = new FileInfo("tempversion.txt");
                        versionInfo.MoveTo("version.txt");
                        DownloadGame();
                    }
                    else
                    {
                        string oldVersionText = File.ReadAllText("version.txt");
                        if(oldVersionText != versionLine)
                        {
                            File.Delete("version.txt");
                            FileInfo versionInfo = new FileInfo("tempversion.txt");
                            versionInfo.MoveTo("version.txt");
                            DownloadGame();
                        }
                        else
                        {
                            OpenGame();
                        }
                    }
                }
            }
        }

        private static void OpenGame()
        {
            Process.Start($@"{documentsPath}\Build\AutomaticUpdater.exe");
        }

        private static void DownloadGame()
        {
            string[] lines = File.ReadAllLines(@"config.txt");

            foreach (var line in lines)
            {
                if (line.Contains("DOWNLOAD_URL="))
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(line.Replace("DOWNLOAD_URL=", ""), "build.zip");
                    if (Directory.Exists($@"{documentsPath}\build"))
                    {
                        Directory.Delete($@"{documentsPath}\build", true);
                    }

                    ZipFile.ExtractToDirectory("build.zip", $@"{documentsPath}");
                    File.Delete("build.zip");
                }
                if (line.Contains("FILE_TO_RUN="))
                {
                    if (File.Exists($@"{documentsPath}\Build\{line.Replace("FILE_TO_RUN=", "")}"))
                    {
                        OpenGame();
                    }
                }
            }
        }
    }
}
