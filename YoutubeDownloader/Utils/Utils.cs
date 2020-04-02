using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using NYoutubeDL;

namespace YoutubeAPI.Utils
{
    public static class Utils
    {
        public static MemoryStream CreateZipFile(Dictionary<string, MemoryStream> files, string format)
        {
            var zipStream = new MemoryStream();
            using (var archiveFile = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (KeyValuePair<string, MemoryStream> vid in files)
                {
                    var newFile = archiveFile.CreateEntry($"{Path.GetFileName(vid.Key)}");
                    
                    using (var entryStream = newFile.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        entryStream.Write(vid.Value.ToArray());
                    }
                }
               
            }
            zipStream.Seek(0, SeekOrigin.Begin);
            return zipStream;
            
        }

        public static MemoryStream CreateZipFile(string directory, string format){
            Dictionary<string, MemoryStream> files = new Dictionary<string, MemoryStream>();
            foreach(var file in Directory.GetFiles(directory)){
                files.Add(Path.GetFileName(file), new MemoryStream(File.ReadAllBytes(file)));
            };
            return CreateZipFile(files, format);
        }

        public static async Task<string> DownloadMP3Async(string url){
            var youtubeDL = new YoutubeDL();
            string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmssffff");
            youtubeDL.Options.FilesystemOptions.Output = fileName + @".%(ext)s";
            youtubeDL.VideoUrl = url;
            youtubeDL.Options.PostProcessingOptions.ExtractAudio = true;
            youtubeDL.Options.PostProcessingOptions.AudioFormat = NYoutubeDL.Helpers.Enums.AudioFormat.mp3;
            youtubeDL.Options.PostProcessingOptions.AudioQuality = "0";
            await youtubeDL.DownloadAsync();
            return fileName + ".mp3"; 
        }

        public static async Task<string> DownloadMP4Async(string url){
            var youtubeDL = new YoutubeDL();
            string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmssffff");
            youtubeDL.Options.FilesystemOptions.Output = fileName + @".%(ext)s";
            youtubeDL.Options.VideoFormatOptions.Format = NYoutubeDL.Helpers.Enums.VideoFormat.best;
            youtubeDL.Options.PostProcessingOptions.RecodeFormat = NYoutubeDL.Helpers.Enums.VideoFormat.mp4;
            youtubeDL.VideoUrl = url;
            await youtubeDL.DownloadAsync();
            return fileName + ".mp4"; 
        }

        public static async Task<MemoryStream> DownloadPlaylistMP3Async(string url){
            var youtubeDL = new YoutubeDL();
            string directoryName = DateTime.Now.ToString("yyyyMMdd_HHmmssffff");
            System.IO.Directory.CreateDirectory(directoryName);
            youtubeDL.Options.FilesystemOptions.Output = Path.Combine(directoryName, @"%(playlist_index)s-%(title)s.%(ext)s");
            youtubeDL.VideoUrl = url;
            youtubeDL.Options.PostProcessingOptions.ExtractAudio = true;
            youtubeDL.Options.PostProcessingOptions.AudioFormat = NYoutubeDL.Helpers.Enums.AudioFormat.mp3;
            youtubeDL.Options.PostProcessingOptions.AudioQuality = "0";
            await youtubeDL.DownloadAsync();
            var files = CreateZipFile(directoryName, "mp3");
            Directory.Delete(directoryName, true);
            return files;

        }
    }

}
