using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;
using MusicHunterServer.Data;
using MusicHunterServer.Models;
using MusicHunterServer.Utils;
using Newtonsoft.Json;
using AudioGistogrammer;


namespace MusicHunterServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {

        private readonly ILogger<Program> _logger;
        private IWebHostEnvironment _appEnvironment;
        private AppDbContext dbContext;
        public UploadController(ILogger<Program> logger, IWebHostEnvironment appEnvironment, AppDbContext dbContext)
        {
            this._logger = logger;
            this._appEnvironment = appEnvironment;
            this.dbContext = dbContext;
        }

        [Route("/upload/track")]
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<string> UploadTrackAsync(List<IFormFile> files)
        {
            string hashString = "";
            string extension = "";
            long size = files.Sum(f => f.Length);
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    extension = Path.GetExtension(formFile.FileName);
                    hashString = Utils.Hasher.GetHashString(formFile.FileName + DateTime.Now.ToString());
                    var filePath = Directory.GetCurrentDirectory() + @"\music\" + hashString + extension;
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    Gistogrammer trackHistogram = new Gistogrammer(filePath);
                    trackHistogram.calculateValues(702);
                    float [] points = trackHistogram.getValuesInPersent(1);
                    string jsonPoints = JsonConvert.SerializeObject(new { pointArray = points });
                    System.IO.File.WriteAllText(Directory.GetCurrentDirectory() + @"\histograms\" + hashString + ".histogram",jsonPoints);
                    _logger.LogInformation($"FILE: {formFile.FileName} was uploaded and saved");

                }
            }
            dbContext.SaveChanges();
            return JsonConvert.SerializeObject(new { message = "File was uploaded", hashUrl = hashString + extension, status = true });
        }


        [Route("/upload/playlist")]
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<string> UploadPlaylistAsync(List<IFormFile> files)
        {
            string hashString = "";
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    hashString = Utils.Hasher.GetHashString(formFile.FileName + DateTime.Now.ToString());
                    var filePath = Directory.GetCurrentDirectory() + @"\music\" + hashString + Path.GetExtension(formFile.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            _logger.LogInformation($"TRACKS WERE UPLOADED AND SAVED [PLAYLIST] HASH: {hashString}");
            dbContext.SaveChanges();
            return JsonConvert.SerializeObject(new { message = "File(s) was uploaded", hash = hashString, status = true });
        }

        [Route("/upload/image")]
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<string> UploadImageAsync(List<IFormFile> files)
        {
            string hashString = "";
            string extension = "";
            long size = files.Sum(f => f.Length);
            _logger.LogInformation("FILE [IMAGE] UPLOAD REQUEST");

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    hashString = Utils.Hasher.GetHashString(formFile.FileName + DateTime.Now.ToString());
                    extension = Path.GetExtension(formFile.FileName);
                    var filePath = Directory.GetCurrentDirectory() + @"\images\" + hashString + extension;
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            dbContext.SaveChanges();
            return JsonConvert.SerializeObject(new { message = "Image file was uploaded", hashUrl = hashString + extension, status = true });
        }

    }
}