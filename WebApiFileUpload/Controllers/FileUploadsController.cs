using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using WebApiFileUpload.Models;
using System.IO;

namespace WebApiFileUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadsController : ControllerBase
    {
        public static IWebHostEnvironment _webHostEnvironment;

        public FileUploadsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        //https://localhost:5001/api/FileUploads
        //body form-data key=>file 
        [HttpPost]
        public string Post([FromForm]FileUpload objectFile)
        {
            try
            {
                if (objectFile.files.Length > 0)
                {
                    string path = _webHostEnvironment.WebRootPath + "\\uploads\\";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream fileStream = System.IO.File.Create(path+objectFile.files.FileName))
                    {
                        objectFile.files.CopyTo(fileStream);
                        fileStream.Flush();
                        return "upload.";
                    }
                }
                else
                {
                    return "upload faild.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        private readonly static Dictionary<string, string> _contentTypes = new Dictionary<string, string>
        {
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"},
            {".zip", "application/zip"}
        };

        //https://localhost:5001/api/FileUploads/{fileName}
        [HttpGet("{fileName}")]
        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }

            var path = _webHostEnvironment.WebRootPath + "\\uploads\\" + fileName;
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);

            // 回傳檔案到 Client 需要附上 Content Type，否則瀏覽器會解析失敗。
            return new FileStreamResult(memoryStream, _contentTypes[Path.GetExtension(path).ToLowerInvariant()]);
        }
    }
}
