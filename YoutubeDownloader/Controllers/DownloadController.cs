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

namespace YoutubeDownloader.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {

        [HttpGet("{url}")]
        public async Task<IActionResult> Download(string url, string format)
        {
            var client = new YoutubeClient();
            var id = YoutubeClient.ParseVideoId(WebUtility.UrlDecode(url));
            var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(id);
            var video = await client.GetVideoAsync(id);

            MediaStreamInfo streamInfo = streamInfoSet.Audio.WithHighestBitrate();
            var tempStream = new MemoryStream();

            await client.DownloadMediaStreamAsync(streamInfo, tempStream);
            tempStream.Seek(0, SeekOrigin.Begin);
            var ext = streamInfo.Container.GetFileExtension();

            return new FileStreamResult(tempStream, "application/octet-stream") { FileDownloadName = $"descarga.mp3"};
        }

    }
}