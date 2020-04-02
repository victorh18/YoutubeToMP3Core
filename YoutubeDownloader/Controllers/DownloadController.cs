using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;   
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using YoutubeAPI.Enums;
using YoutubeAPI.Utils;
using NYoutubeDL;

namespace YoutubeDownloader.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {

        [HttpGet]
        //public async Task<IActionResult> Download(string url = null, FormatoDescarga format = FormatoDescarga.mp3, TimeSpan? TiempoInicio = null, TimeSpan? TiempoFin = null)
        public async Task<IActionResult> Download(string url = null, FormatoDescarga format = FormatoDescarga.mp3)
        {
            var youtubeDL = new YoutubeDL();
            var metaClient = new YoutubeClient();

            string mediaUrl = WebUtility.UrlDecode(url);
            var mediaMetadata = await metaClient.GetVideoAsync(YoutubeClient.ParseVideoId(mediaUrl));
            Task<string> fileTask = null;

            switch(format){
                case FormatoDescarga.mp3:
                    fileTask = Utils.DownloadMP3Async(mediaUrl);
                    break;
                case FormatoDescarga.mp4:
                    fileTask = Utils.DownloadMP4Async(mediaUrl);
                    break;

            }
            MemoryStream media = new MemoryStream(System.IO.File.ReadAllBytes(fileTask.Result));
            media.Seek(0, SeekOrigin.Begin);
            System.IO.File.Delete(fileTask.Result);
            return new FileStreamResult(media, "application/octet-stream") { FileDownloadName = (mediaMetadata.Title + Path.GetExtension(fileTask.Result))};
            
             
        }

        [HttpGet]
        [Route("Playlist")]
        public async Task<IActionResult> DownloadPlaylist(string url = null, FormatoDescarga format = FormatoDescarga.mp3, TimeSpan? TiempoInicio = null, TimeSpan? TiempoFin = null)
        {
            var client = new YoutubeClient();
            var playlistId = YoutubeClient.ParsePlaylistId(WebUtility.UrlDecode(url));
            //var playlistId = "PLQLqnnnfa_fAkUmMFw5xh8Kv0S5voEjC9";
            var playlist = await client.GetPlaylistAsync(playlistId);

            var vids = new Dictionary<string, MemoryStream>();
            
            foreach (var vid in playlist.Videos)
            {
                
                var tempStream = new MemoryStream();
                var infoSet = await client.GetVideoMediaStreamInfosAsync(vid.Id);
                MediaStreamInfo streamInfo;
                switch (format)
                {
                    case FormatoDescarga.mp4:
                        streamInfo = infoSet.Muxed.WithHighestVideoQuality();
                        break;
                    case FormatoDescarga.mp3:
                        streamInfo = infoSet.Audio.WithHighestBitrate();
                        break;
                    default:
                        format = FormatoDescarga.mp3;
                        streamInfo = infoSet.Audio.WithHighestBitrate();
                        break;
                }

                await client.DownloadMediaStreamAsync(streamInfo, tempStream);

                vids.Add(vid.Title, tempStream);
            }

            var archivo = Utils.CreateZipFile(vids);
            
            archivo.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(archivo, "application/octet-stream") { FileDownloadName = $"{playlist.Title}.zip" };
        }

    }
}