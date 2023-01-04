using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServiceBus.Infraestructure.CommonServices;
using ServiceBusSenderApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ServiceBusSenderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobController : Controller
    {
        private readonly IAzureBlobStorage _azureBlobStorage;
        private readonly IConfiguration _configuration;
        private readonly string container = string.Empty;
        /// <summary>
        /// contructor
        /// </summary>
        /// <param name="azureBlobStorage">Blob Interface</param>
        /// <param name="configuration">Config</param>
        public BlobController(IAzureBlobStorage azureBlobStorage, IConfiguration configuration)
        {
            _azureBlobStorage = azureBlobStorage;
            _configuration = configuration;
            container = _configuration["ContainerName"] ?? _configuration["ContainerName"].ToString();
        }
        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadAsync(ICollection<IFormFile> files,string description)
        {
            try
            {                
                ICollection<dynamic> result = default;
                foreach (var formFile in files)
                {
                    if (formFile.Length <= 0)
                        continue;

                    var extension = Path.GetExtension(formFile.FileName);
                    await using var stream = formFile.OpenReadStream();
                    result.Add(await _azureBlobStorage.UploadBlobAsync(formFile.FileName, formFile, container));
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(400, e.Message);
            }
         
        }
        [HttpPost("uploadFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadFileAsync([FromBody] SuraDocument document)
        {
            try
            {                
                byte[] bytes = Convert.FromBase64String(document.FileSura);
                var stream = new MemoryStream(bytes);
                var result = await _azureBlobStorage.UploadBlobAsync(document.FileNameSura, stream, container, false);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(400, e.Message);
            }
            
        }
        [HttpGet("download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadAsync([FromQuery] string fileName)
        {
            try
            {
                var result = await _azureBlobStorage.GetBlobBytesAsync(fileName, container);
                string base64String = string.Empty;
                using (var streamReader = new StreamReader(result.Content))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        streamReader.BaseStream.CopyTo(ms);
                        base64String=Convert.ToBase64String(ms.ToArray());
                    }
                }
             
                return Ok(base64String);
            }
            catch (Exception e)
            {
                return StatusCode(400, e.Message);
            }
           
        }
    }
}
