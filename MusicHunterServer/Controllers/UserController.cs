using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicHunterServer.Data;
using MusicHunterServer.Models;
using MusicHunterServer.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MusicHunterServer.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ILogger<Program> _logger;
        private readonly UserDbContext _dbContext;
        private readonly TokenManager _tokenManager;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly DefaultContractResolver _contractResolver;

        public UserController(UserDbContext dbContext, ILogger<Program> logger, IOptions<AppSettings> appSettings)
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._appSettings = appSettings;
            this._tokenManager = new TokenManager(this._appSettings.Value.Secret);
            this._contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
        }

        [Route("api/user/addtrack")]
        [HttpPost]
        public async Task<string> UserTrackAdd(TrackToUser track)
        {
            _logger.LogInformation($"[TrackToUser controller request] = trackHash: {track.TrackHash}  userHash: {track.UserHash}");
            _dbContext.TrackUserRelations.Add(track);
            await _dbContext.SaveChangesAsync();
            return JsonConvert.SerializeObject(new { message = $"Track was added to User: {track.UserHash}", status = true });
        }

        [Route("api/user/addplaylist")]
        [HttpPost]
        public async Task<string> UserPlaylistAdd(PlaylistToUser playlist)
        {
            _logger.LogInformation($"[PlaylistToUser controller request] = playlistHash: {playlist.PlaylistHash}  userHash: {playlist.UserHash}");
            _dbContext.PlaylistUserRelations.Add(playlist);
            await _dbContext.SaveChangesAsync();
            return JsonConvert.SerializeObject(new { message = $"Playlist was added to User: {playlist.UserHash}", status = true });
        }
        [Route("api/user/getTracks")]
        [HttpGet]
        public string GetTracksOfUser(string userHash)
        {
            _logger.LogInformation($"Get tracks [userHash  = {userHash}]");
            var tracks = _dbContext.Tracks.FromSqlRaw("select T.Id,T.Artist,T.HashUrl,T.ImageUrl,T.Name,T.OwnerId from TrackUserRelations join Tracks as T on TrackHash = T.HashUrl where UserHash like @userHash", new SqlParameter("@userHash", userHash));
            foreach (var track in tracks)
            {
                track.Histogram = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + @"\histograms\" + Path.GetFileNameWithoutExtension(track.HashUrl) + ".histogram");
                track.HashUrl = "https://localhost:5001/music/" + track.HashUrl;
                track.ImageUrl = "https://localhost:5001/images/" + track.ImageUrl;
            }
            return JsonConvert.SerializeObject(new
            {
                messsage = $"TRACKS RECEIVED FOR {userHash}",
                tracks = JsonConvert.SerializeObject(tracks, new JsonSerializerSettings
                {
                    ContractResolver = this._contractResolver,
                    Formatting = Formatting.Indented
                }),
                status = true
            });
        }

        [Route("api/user/getPlaylists")]
        [HttpGet]
        public string GetAlbumsOfUser(string userHash)
        {
            _logger.LogInformation($"Get playlists [userHash  = {userHash}]");
            var playlists = _dbContext.Playlists.FromSqlRaw("select P.Id, P.Hash, P.ImageUrl, P.Name, P.OwnerId,P.Type from PlaylistUserRelations as PP join Playlists as P on PP.PlaylistHash = P.Hash where UserHash like @userHash", new SqlParameter("@userHash", userHash));
            foreach (var playlist in playlists)
            {
                var tracks = _dbContext.Tracks.FromSqlRaw("select T.Id,T.Artist,T.HashUrl,T.ImageUrl,T.Name,T.OwnerId from PlaylistRelations as PR join Tracks as T on T.HashUrl = PR.TrackHashUrl where PR.PlaylistHash like @playlistHash;", new SqlParameter("@playlistHash", playlist.Hash));
                playlist.Tracks = tracks.ToList();
                foreach (var track in playlist.Tracks)
                {
                    track.Histogram = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + @"\histograms\" + Path.GetFileNameWithoutExtension(track.HashUrl) + ".histogram");
                    track.HashUrl = "https://localhost:5001/music/" + track.HashUrl;
                    track.ImageUrl = "https://localhost:5001/images/" + track.ImageUrl;
                }
            }



            return JsonConvert.SerializeObject(new
            {
                messsage = $"Playlist[GET] RESPONSE USERHASH: {userHash}",
                playlists = JsonConvert.SerializeObject(playlists, new JsonSerializerSettings
                {
                    ContractResolver = this._contractResolver,
                    Formatting = Formatting.Indented
                }),
                status = true
            });
        }

        [Route("api/public/userByToken")]
        [HttpGet]
        public string GetUserByToken(string token)
        {
            _logger.LogInformation($"[GetUserByToken] request");
            string email = this._tokenManager.ValidateAndGetEmail(token);
            if (email == null)
            {
                _logger.LogInformation($"[GetUserByToken] - {email} : Invalid token");
                return JsonConvert.SerializeObject(new { messsage = $"Invalid token", status = false });
            }
            var user = _dbContext.Users.Where(user => user.Email == email).FirstOrDefault();
            if (user == null)
            {
                _logger.LogInformation($"User with email {email} was not founded");
                return JsonConvert.SerializeObject(new { messsage = $"Invalid token, user {email} was not founded", status = false });
            }
            return JsonConvert.SerializeObject(new
            {
                messsage = $"Success, user was founded",
                status = true,
                user = JsonConvert.SerializeObject(user, new JsonSerializerSettings
                {
                    ContractResolver = this._contractResolver,
                    Formatting = Formatting.Indented
                })
            });
        }
    }
}