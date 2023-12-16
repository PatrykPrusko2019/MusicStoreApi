﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStoreApi.Entities;
using MusicStoreApi.Models;
using MusicStoreApi.Services;
using System.Data;

namespace MusicStoreApi.Controllers
{
    [Route("api/artist/{artistId}/album/{albumId}/song")]
    [ApiController]
    [Authorize]
    public class SongController : ControllerBase
    {
        private readonly ISongService songService;

        public SongController(ISongService songService)
        {
            this.songService = songService;
        }

        [Authorize(Roles = "Admin,PremiumUser")]
        [HttpPost]
        public ActionResult Create([FromRoute]int artistId, [FromRoute]int albumId, [FromBody] CreateSongDto songDto)
        {
            var songId = songService.Create(artistId, albumId, songDto);

            return Created($"/api/artist/{artistId}/album/{albumId}/song/{songId}", null);
        }

        [Authorize(Roles = "Admin,PremiumUser")]
        [HttpPut("{songId}")]
        public ActionResult Update([FromRoute]int artistId, [FromRoute]int albumId, [FromRoute]int songId, [FromBody]UpdateSongDto updateSongDto)
        {
            songService.Update(artistId, albumId, songId, updateSongDto);

            return Ok($"/api/artist/{artistId}/album/{albumId}/song/{songId}");
        }

        [Authorize(Roles = "Admin,PremiumUser")]
        [HttpDelete("{songId}")]
        public ActionResult Delete([FromRoute]int artistId, [FromRoute]int albumId, [FromRoute]int songId)
        {
            songService.Delete(artistId, albumId, songId);

            return Ok($"/api/artist/{artistId}/album/{albumId}/song/{songId}");
        }

        [HttpGet]
        public ActionResult<List<SongDto>> GetAll([FromRoute]int artistId, [FromRoute]int albumId)
        {
            var songs = songService.GetAll(artistId, albumId);

            return Ok(songs);
        }

        [HttpGet("{songId}")]
        public ActionResult GetById([FromRoute]int artistId, [FromRoute]int albumId, [FromRoute]int songId)
        {
            var song = songService.GetById(artistId, albumId, songId);

            return Ok(song);
        }


    }
}
