using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicHunterServer.Data;
using MusicHunterServer.Models;
using Newtonsoft.Json;

namespace MusicHunterServer.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ILogger<Program> _logger;
        private readonly UserDbContext _dbContext;
        public UserController(UserDbContext dbContext, ILogger<Program> logger)
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        [Route("api/user/addtrack")]
        [HttpPost]
        public async Task<string> UserTrackAdd(TrackToUser track)
        {
            _logger.LogInformation($"[TrackToUser controller request] = trackHash: {track.TrackHash}  userHash: {track.UserHash}");
            _dbContext.TrackUserRelations.Add(track);
            await _dbContext.SaveChangesAsync();
            return JsonConvert.SerializeObject(new { message = "Track was added to User", status = true});
        }

        [Route("api/user/addplaylist")]
        [HttpPost]
        public async Task<string> UserPlaylistAdd(PlaylistToUser playlist)
        {
            _logger.LogInformation($"[TrackToUser controller request] = trackHash: {playlist.PlaylistHash}  userHash: {playlist.UserHash}");
            _dbContext.PlaylistUserRelations.Add(playlist);
            await _dbContext.SaveChangesAsync();
            return JsonConvert.SerializeObject(new { message = "Track was added to User", status = true });
        }
        [Route("api/user/getTracks")]
        [HttpGet]
        public string GetTracksOfUser(string userHash)
        {
            _logger.LogInformation($"Get tracks [userHash  = {userHash}]");
            var tracks = _dbContext.Tracks.FromSqlRaw("select T.Id,T.Artist,T.HashUrl,T.ImageUrl,T.Name,T.OwnerId from TrackUserRelations join Tracks as T on TrackHash = T.HashUrl where UserHash like @userHash", new SqlParameter("@userHash", userHash));
             
            return JsonConvert.SerializeObject(new { messsage = "Tracks of ", tracks = JsonConvert.SerializeObject(tracks), status = true});
        }
    }
}