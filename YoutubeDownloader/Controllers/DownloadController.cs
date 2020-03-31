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

namespace YoutubeDownloader.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Download(string url, FormatoDescarga format, string TiempoInicio, string TiempoFin)
        {
            try
            {
                var client = new YoutubeClient();
                var id = YoutubeClient.ParseVideoId(WebUtility.UrlDecode(url));
                var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(id);
                var video = await client.GetVideoAsync(id);
                var tempStream = new MemoryStream();
                MediaStreamInfo streamInfo;

                switch (format)
                {
                    case FormatoDescarga.mp4:
                        streamInfo = streamInfoSet.Muxed.WithHighestVideoQuality();
                        break;
                    case FormatoDescarga.mp3:
                        streamInfo = streamInfoSet.Audio.WithHighestBitrate();
                        break;
                    default:
                        format = FormatoDescarga.mp3;
                        streamInfo = streamInfoSet.Audio.WithHighestBitrate();
                        break;
                }
                await client.DownloadMediaStreamAsync(streamInfo, tempStream);
                tempStream.Seek(0, SeekOrigin.Begin);
                var ext = streamInfo.Container.GetFileExtension();

                return new FileStreamResult(tempStream, "application/octet-stream") { FileDownloadName = $"{video.Title}.{format}" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }  
        }

    }
}