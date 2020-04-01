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

namespace YoutubeDownloader.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Download(string url = null, FormatoDescarga format = FormatoDescarga.mp3, TimeSpan? TiempoInicio = null, TimeSpan? TiempoFin = null)
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