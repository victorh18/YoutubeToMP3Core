using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
