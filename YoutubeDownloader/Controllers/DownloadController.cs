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
        public async Task<IActionResult> DownloadPlaylist(string url = null, FormatoDescarga format = FormatoDescarga.mp3)
        {
            return new FileStreamResult(await Utils.DownloadPlaylistMP3Async(url), "application/octet-stream") { FileDownloadName = $"files.zip"};
        }

    }
}