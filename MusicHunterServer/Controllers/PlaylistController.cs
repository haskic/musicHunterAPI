using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicHunterServer.Data;
using MusicHunterServer.Models;
using Newtonsoft.Json;

namespace MusicHunterServer.Controllers
{
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private AppDbContext _dbContext;
        private ILogger<Program> _logger;
        public PlaylistController(AppDbContext dbContext, ILogger<Program> logger)
        {
            this._dbContext = dbContext;
            this._logger = logger;

        }

        [Route("/playlist/add")]
        [HttpPost]
        public async Task<string> AddPlaylist([FromBody] Playlist playlist)
        {
            string playlistHash = Utils.Hasher.GetHashString(playlist.Name + new DateTime().ToString());
            _logger.LogInformation("Add Playlist Request: " + playlist.Name);
            playlist.Hash = playlistHash;
            _dbContext.Add(playlist);
            await _dbContext.SaveChangesAsync();

            return JsonConvert.SerializeObject(new { message = "Playlist was saved", hash = playlistHash, success = true, status = true });
        }
        [Route("playlist/relations/add")]
        [HttpPost]
        public async Task<string> AddRelations(IList<PlaylistRelation> relations)
        {
            foreach (var relation in relations)
            {
                _logger.LogInformation("Add Relation between playlistHash: " + relation.PlaylistHash + " && " + " trackHash: " + relation.TrackHashUrl);
                _dbContext.PlaylistRelations.Add(relation);
            }
            await _dbContext.SaveChangesAsync();
            return JsonConvert.SerializeObject(new { message = "Relations was saved", status = true });
        }
        [Route("playlist/relation/add")]
        [HttpPost]
        public async Task<string> AddRelation([FromBody] PlaylistRelation relation)
        {

            _logger.LogInformation("Add Relation between playlistHash: " + relation.PlaylistHash + " && " + " trackHash: " + relation.TrackHashUrl);
            _dbContext.PlaylistRelations.Add(relation);
            await _dbContext.SaveChangesAsync();
            return JsonConvert.SerializeObject(new { message = "Relation was saved", status = true });
        }
    }
}