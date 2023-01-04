using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServiceBus.Infraestructure.CommonServices;
using ServiceBusMessaging;
using ServiceBusSenderApi.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBusSenderApi.Controllers
{
    [Route("api/[controller]",Name = "Documents")]
    [ApiController]
    public class DocumentsController : Controller
    {
        private readonly ServiceBusSender _serviceBusSender;
        private readonly IAzureBlobStorage _azureBlobStorage;      
        private readonly string container = string.Empty;
        /// <summary>
        /// Payload Documents manager COnstructor
        /// </summary>
        /// <param name="serviceBusSender">Obj Service Bus</param>
        /// <param name="azureBlobStorage">Obj Blob manager</param>
        /// <param name="configuration">Config</param>
        public DocumentsController(ServiceBusSender serviceBusSender, IAzureBlobStorage azureBlobStorage, IConfiguration configuration)
        {
            _serviceBusSender = serviceBusSender;
            _azureBlobStorage = azureBlobStorage;
            IConfiguration _configuration = configuration;
            container = _configuration["ContainerName"] ?? _configuration["ContainerName"].ToString();
        }

        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public ActionResult<List<Payload>> Get()
        //{
        //    return Ok(data);
        //}

        [HttpGet("{idDocument}",Name ="Consultar Documento")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Get(string idDocument)
        {
            try
            {
                if (string.IsNullOrEmpty(idDocument))
                {
                    return BadRequest();
                }               
                await _serviceBusSender.SendMessageSolicitaDoc(new Documento
                {
                    documentoID = idDocument
                }).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(409, e.Message);
            }       
        }

        [HttpPost]
        [ProducesResponseType(typeof(Payload), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Payload), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Payload), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody][Required] Payload request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (data.Any(d => d.Id == request.Id))
                    {
                        return Conflict($"data with id {request.Id} already exists");
                    }

                    data.Add(request);                    
                    byte[] bytes = Convert.FromBase64String(request.FileSura);
                    var stream = new MemoryStream(bytes);
                    request.documentoID = Guid.NewGuid().ToString();
                    string filenameBlob = $"{request.documentoID}.{request.FileExtension}";
                    Azure.Response<BlobContentInfo> blobs = await _azureBlobStorage.UploadBlobAsync(filenameBlob, stream, container, false);
                    string resultUrl = _azureBlobStorage.GetBlobUrlAsync(container, filenameBlob);
                    // Send this to the bus for the other services
                    await _serviceBusSender.SendMessage(new MyPayload
                    {
                        Id = request.Id,
                        documentoID = request.documentoID,
                        Name = request.Name,
                        messageId = Guid.NewGuid().ToString(),
                        Delete = false,
                        idDNI = request.idDNI,
                        FileNameSura = request.FileNameSura,
                        FileSura = resultUrl,
                        FileExtension = request.FileExtension,
                        Description = request.Description
                    }).ConfigureAwait(false);
                    return Ok(request);
                }
                else {
                    return StatusCode(400, string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
                }     
            }
            catch (Exception e)
            {
                return StatusCode(409, e.Message);
            }

        }

        //[HttpPut]
        //[Route("{id}")]
        //[ProducesResponseType(typeof(Payload), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        //[ProducesDefaultResponseType]
        //public async Task<IActionResult> Update(int id, [FromBody][Required] Payload request)
        //{
        //    try
        //    {
        //        if (!data.Any(d => d.Id == request.Id))
        //        {
        //            return NotFound($"data with id {id} does not exist");
        //        }

        //        var item = data.First(d => d.Id == id);
        //        item.Id = request.Id;
        //        item.Name = request.Name;
        //        item.documentoID = request.documentoID;
        //        item.messageId = request.messageId;
        //        item.idDNI = request.idDNI;
        //        item.FileNameSura = request.FileNameSura;
        //        item.FileSura = "";
        //        item.FileExtension = request.FileExtension;
        //        item.Description = request.Description;
        //        // Send this to the bus for the other services
        //        await _serviceBusSender.SendMessage(new MyPayload
        //        {
        //            documentoID = request.documentoID,
        //            Name = request.Name,
        //            messageId = request.messageId,
        //            Delete = false,
        //            idDNI = request.idDNI,
        //            FileNameSura = request.FileNameSura,
        //            FileSura = "",
        //            FileExtension = request.FileExtension,
        //            Description = request.Description
        //        }).ConfigureAwait(false);

        //        return Ok(request);
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(400, e.Message);
        //    }

        //}

        //[HttpDelete]
        //[Route("{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status409Conflict)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesDefaultResponseType]
        //public async Task<IActionResult> Delete([FromRoute] int id)
        //{
        //    if (id == 0)
        //    {
        //        return BadRequest();
        //    }

        //    if (!data.Any(d => d.Id == id))
        //    {
        //        return NotFound($"data with id {id} does not exist");
        //    }

        //    var item = data.First(d => d.Id == id);
        //    data.Remove(item);

        //    // Send this to the bus for the other services
        //    await _serviceBusSender.SendMessage(new MyPayload
        //    {
        //        documentoID = item.documentoID,
        //        Name = item.Name,
        //        Delete = true
        //    }).ConfigureAwait(false);

        //    return Ok();
        //}

        private static readonly List<Payload> data = new List<Payload>
        {
            new Payload{ Id=1, documentoID="rsdefre", Name="wow"},
            new Payload{ Id=2, documentoID="dedgthrg5453", Name="not so bad"},
        };
    }
}
