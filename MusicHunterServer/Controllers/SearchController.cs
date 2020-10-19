using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicHunterServer.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MusicHunterServer.Controllers
{
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly SearchDbContext _dbContext;
        private readonly ILogger<SearchController> _logger;
        private readonly DefaultContractResolver _contractResolver;


        public SearchController(SearchDbContext dbContext, ILogger<SearchController> logger)
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
        }


        [Route("api/search/tracks")]
        [HttpGet]
        public string GetTracksOfUser([FromQuery]string line)
        {
            _logger.LogInformation($"search request for line: ${line}");


            var tracks = _dbContext.Tracks.Where((track) => track.Name.StartsWith(line));
            foreach (var track in tracks)
            {
                track.Histogram = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + @"\histograms\" + Path.GetFileNameWithoutExtension(track.HashUrl) + ".histogram");
                track.HashUrl = "https://localhost:5001/music/" + track.HashUrl;
                track.ImageUrl = "https://localhost:5001/images/" + track.ImageUrl;
            }
            //var tracks = _dbContext.Tracks.FromSqlRaw("select T.Id,T.Artist,T.HashUrl,T.ImageUrl,T.Name,T.OwnerId from TrackUserRelations join Tracks as T on TrackHash = T.HashUrl where UserHash like @userHash", new SqlParameter("@userHash", userHash));
            //foreach (var track in tracks)
            //{
            //    track.Histogram = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + @"\histograms\" + Path.GetFileNameWithoutExtension(track.HashUrl) + ".histogram");
            //    track.HashUrl = "https://localhost:5001/music/" + track.HashUrl;
            //    track.ImageUrl = "https://localhost:5001/images/" + track.ImageUrl;
            //}
            return JsonConvert.SerializeObject(new
            {
                messsage = $"TRACKS FOR search line {line}",
                tracks = JsonConvert.SerializeObject(tracks, new JsonSerializerSettings
                {
                    ContractResolver = this._contractResolver,
                    Formatting = Formatting.Indented
                }),
                status = true
            });
        }
    }
}