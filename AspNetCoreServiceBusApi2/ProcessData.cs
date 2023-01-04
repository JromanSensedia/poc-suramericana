
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServiceBusMessaging;
using ServiceBusReceiverApi.Model;
using System;
using System.Threading.Tasks;

namespace ServiceBusReceiverApi
{
    public class ProcessData : IProcessData
    {
        private IConfiguration _configuration;
        public ProcessData(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task Process(MyPayload myPayload)
        {
            using (var payloadMessageContext =
                new PayloadMessageContext(_configuration.GetConnectionString("DefaultConnection")))
            {
                await payloadMessageContext.AddAsync(new Payload
                {
                    Name = myPayload.Name,
                    documentoID = myPayload.documentoID,
                    Created = DateTime.UtcNow,
                    messageId = myPayload.messageId,
                    Description = myPayload.Description,
                    idDNI = myPayload.idDNI,
                    FileSura = myPayload.FileSura,
                    FileExtension = myPayload.FileExtension,
                    FileNameSura = myPayload.FileNameSura,
                }).ConfigureAwait(false);

                await payloadMessageContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
        public async Task<dynamic> GetDocument(Documento document)
        {
            using (var payloadMessageContext =
             new PayloadMessageContext(_configuration.GetConnectionString("DefaultConnection")))
            {
                var documentOut = await payloadMessageContext.Payloads.FirstOrDefaultAsync(x => x.documentoID == document.documentoID);
                if (documentOut != null) { return documentOut; }
                else
                {
                    document.documentStatus = $"File id: {document.documentoID} not found in Storage.";
                    return document;
                }
            }
        }
    }
}
