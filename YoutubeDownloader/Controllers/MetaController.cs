﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YoutubeExplode;

namespace YoutubeDownloader.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MetaController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetMetadata(string url)
        {
            var client = new YoutubeClient();
            var id = YoutubeClient.ParseVideoId(WebUtility.UrlDecode(url));
            var video = await client.GetVideoAsync(id);
            return Ok(video);
        }

        [HttpPost]
        public async Task<IActionResult> TestThis()
        {
            return Ok("Hummmmm");
        }

        [Route("Playlist")]
        [HttpGet]
        public async Task<IActionResult> GetPlaylistMetadata()
        {
            var client = new YoutubeClient();
            var playlist = await client.GetPlaylistAsync("PLQLqnnnfa_fAkUmMFw5xh8Kv0S5voEjC9");
            return Ok(playlist.Videos);
        }
    }
}