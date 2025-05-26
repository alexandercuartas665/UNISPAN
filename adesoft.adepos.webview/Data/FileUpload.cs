using adesoft.adepos.Extensions;
using adesoft.adepos.webview.Data.DTO.PL;
using adesoft.adepos.webview.Data.Interfaces;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Data.Model.PL;
using BlazorInputFile;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class FileUpload : IFileUpload
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        private readonly ConnectionDB _connectionDB;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileUpload(IWebHostEnvironment webHostEnvironment, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;

            _configuration = configuration;
            _connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (_connectionDB == null)
                _connectionDB = SecurityService.GetConnectionDefault();
            _dbcontext = new AdeposDBContext(_connectionDB.Connection);
        }

        public async Task UploadFile(IFileListEntry file, DTOOrder dtoOrder)
        {
            try
            {
                var imageStorePath = this._configuration.GetValue<string>("Logistics:ImageStorePath");
                var directory = string.Format("{0}/{1}", imageStorePath, dtoOrder.OrderId);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var ext = Path.GetExtension(file.Name);

                var path = string.Format("{0}/{1}", directory, string.Format("{0}{1}", DateTime.UtcNow.Ticks, ext));
                var memStream = new MemoryStream();
                await file.Data.CopyToAsync(memStream);

                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    memStream.WriteTo(fs);
                }

                if (File.Exists(path))
                {
                    var picture = new OrderPicture()
                    {
                        OrderId = dtoOrder.OrderId,
                        Name = file.Name,
                        OrderType = dtoOrder.OrderType,
                        Path = path
                    };

                    _dbcontext.OrderPictures.Add(picture);
                    _dbcontext.SaveChanges();
                    _dbcontext.DetachAll();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
