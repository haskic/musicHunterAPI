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
            _logger.LogInformation("FILE [TRACK] UPLOAD REQUEST");

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    extension = Path.GetExtension(formFile.FileName);
                    hashString = Utils.Hasher.GetHashString(formFile.FileName + DateTime.Now.ToString());
                    var filePath = Directory.GetCurrentDirectory() + @"\music\" + hashString + extension;
                    _logger.LogInformation("FILE NAME = " + formFile.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    Gistogrammer trackHistogram = new Gistogrammer(filePath);
                    trackHistogram.calculateValues(702);
                    float [] points = trackHistogram.getValuesInPersent(1);
                    string jsonPoints = JsonConvert.SerializeObject(new { pointArray = points });
                    System.IO.File.WriteAllText(Directory.GetCurrentDirectory() + @"\histograms\" + hashString + ".histogram",jsonPoints);
                }
            }



            //Message message = new Message();
            //message.ConversationId = 12;
            //message.SenderId = 13;
            //message.CreatedAt = new DateTime(2015, 7, 20);
            //dbContext.Messages.Add(message);
            dbContext.SaveChanges();
            //_logger.LogError("TEST PATH = " + _appEnvironment.WebRootPath);
            //_logger.LogWarning("UPLOAD PATH: " + Directory.GetCurrentDirectory() + @"\music");
            return JsonConvert.SerializeObject(new { message = "file was uploaded", hashUrl = hashString + extension, status = true });
        }


        [Route("/upload/playlist")]
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<string> UploadPlaylistAsync(List<IFormFile> files)
        {
            string hashString = "";
            long size = files.Sum(f => f.Length);
            _logger.LogInformation("FILE [PLAYLIST] UPLOAD REQUEST");

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    hashString = Utils.Hasher.GetHashString(formFile.FileName + DateTime.Now.ToString());
                    var filePath = Directory.GetCurrentDirectory() + @"\music\" + hashString + Path.GetExtension(formFile.FileName);
                    _logger.LogInformation("FILE NAME = " + formFile.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            //Message message = new Message();
            //message.ConversationId = 12;
            //message.SenderId = 13;
            //message.CreatedAt = new DateTime(2015, 7, 20);
            //dbContext.Messages.Add(message);
            dbContext.SaveChanges();
            //_logger.LogError("TEST PATH = " + _appEnvironment.WebRootPath);
            //_logger.LogWarning("UPLOAD PATH: " + Directory.GetCurrentDirectory() + @"\music");
            return JsonConvert.SerializeObject(new { message = "file was uploaded", hash = hashString, status = true });
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
                    _logger.LogInformation("EXTENSION: " + extension);
                    _logger.LogInformation("FILE NAME = " + formFile.FileName);

                    var filePath = Directory.GetCurrentDirectory() + @"\images\" + hashString + extension;


                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            //Message message = new Message();
            //message.ConversationId = 12;
            //message.SenderId = 13;
            //message.CreatedAt = new DateTime(2015, 7, 20);
            //dbContext.Messages.Add(message);
            dbContext.SaveChanges();
            //_logger.LogError("TEST PATH = " + _appEnvironment.WebRootPath);
            //_logger.LogWarning("UPLOAD PATH: " + Directory.GetCurrentDirectory() + @"\music");
            return JsonConvert.SerializeObject(new { message = "image file was uploaded", hashUrl = hashString + extension, status = true });
        }

    }
}