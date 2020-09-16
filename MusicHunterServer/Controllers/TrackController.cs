using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicHunterServer.Data;
using MusicHunterServer.Models;
using Newtonsoft.Json;

namespace MusicHunterServer.Controllers
{
    [ApiController]
    public class TrackController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<Program> _logger;

        public TrackController(AppDbContext dbContext, ILogger<Program> logger)
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }
        [Route("/track/add")]
        [HttpPost]
        public async Task<string> AddTrack([FromBody] Track track)
        {
            _logger.LogInformation("TRACK CONTROLLER[ADD TRACK REQUEST]: " + track.Name);
            _dbContext.Tracks.Add(track);
            await _dbContext.SaveChangesAsync();
            return JsonConvert.SerializeObject( new { message = "Track was saved", status = true});
        }
        [Route("/tracks/add")]
        [HttpPost]
        public async Task<string> AddTracks( IList<Track> tracks)
        {
            foreach (var track in tracks)
            {
                _logger.LogInformation("TRACK CONTROLLER[ADD TRACKS REQUEST]: " + track.Name);
                _dbContext.Tracks.Add(track);
            }

            await _dbContext.SaveChangesAsync();

            return JsonConvert.SerializeObject(new { message = $"Tracks were saved, number of tracks: {tracks.Count}", status = true});
        }
    }
}