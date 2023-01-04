using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Infraestructure.CommonServices
{
    public interface IAzureBlobStorage
    {
        Task<IEnumerable<string>> AllBlobs(string containerName);
        Task<string> GetBlobAsync(string name, string containerName);
        Task<BlobDownloadInfo> GetBlobBytesAsync(string name, string containerName);
        string GetBlobUrlAsync(string containerName, string fileName);
        Task<dynamic> UploadBlobAsync(string fileName, IFormFile file, string containerName);
        Task<dynamic> UploadBlobAsync(string fileName, Stream uploadFileStream, string containerName, bool overwrite);
        Task<List<BlobItem>> ListAllItemsInContainer();
        Task<bool> DeleteBlob(string name, string containerName);
    }
}
