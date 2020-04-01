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
        public static MemoryStream CreateZipFile(Dictionary<string, MemoryStream> vids)
        {
            var zipStream = new MemoryStream();
            using (var archiveFile = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (KeyValuePair<string, MemoryStream> vid in vids)
                {
                    var newFile = archiveFile.CreateEntry($"{vid.Key}.mp4");
                    
                    using (var entryStream = newFile.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        entryStream.Write(vid.Value.ToArray());
                        //streamWriter.Write(vid.Value);
                        //streamWriter.
                    }
                }
               
            }
            return zipStream;
            
        }

        public static async Task<string> DownloadMP3Async(string url){
            var youtubeDL = new YoutubeDL();
            youtubeDL.Options.FilesystemOptions.Output = @"%(title)s.%(ext)s";
            youtubeDL.VideoUrl = url;
            youtubeDL.Options.PostProcessingOptions.ExtractAudio = true;
            youtubeDL.Options.PostProcessingOptions.AudioFormat = NYoutubeDL.Helpers.Enums.AudioFormat.mp3;
            youtubeDL.Options.PostProcessingOptions.AudioQuality = "0";
            await youtubeDL.DownloadAsync();
            return youtubeDL.Info.Title + ".mp3"; 
        }

        public static async Task<string> DownloadMP4Async(string url){
            var youtubeDL = new YoutubeDL();
            youtubeDL.Options.FilesystemOptions.Output = @"%(title)s.%(ext)s";
            youtubeDL.Options.VideoFormatOptions.Format = NYoutubeDL.Helpers.Enums.VideoFormat.best;
            youtubeDL.Options.PostProcessingOptions.RecodeFormat = NYoutubeDL.Helpers.Enums.VideoFormat.mp4;
            youtubeDL.VideoUrl = url;
            Console.WriteLine(youtubeDL.PrepareDownload());
            await youtubeDL.DownloadAsync();
            return youtubeDL.Info.Title + ".mp4"; 
        }
    }
}
