using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;

namespace ServiceBus.Infraestructure.CommonServices
{
    public class AzureBlobStorage: IAzureBlobStorage
    {
        private readonly BlobServiceClient _blobClient;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blobClient">Blob manager</param>
        public AzureBlobStorage(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }
        /// <summary>
        /// All Blobs
        /// </summary>
        /// <param name="containerName">Name Container</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> AllBlobs(string containerName)
        {
            var containerClient = _blobClient.GetBlobContainerClient(containerName);
            var files = new List<string>();
            var blobs = containerClient.GetBlobsAsync();
            await foreach (var item in blobs)   
            {
                files.Add(item.Name);
            }
            return files;
        }
        /// <summary>
        /// Get blob Async
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="containerName">Name Container</param>
        /// <returns></returns>
        public async Task<string> GetBlobAsync(string name, string containerName)
        {
            var containerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(name);

            return await Task.Run(() => blobClient.Uri.AbsoluteUri);
        }
        /// <summary>
        /// Get blob bytes
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="containerName">Name Container</param>
        /// <returns></returns>
        public async Task<BlobDownloadInfo> GetBlobBytesAsync(string name, string containerName)
        {
            var containerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(name);
            var blobDownloadInfo = await blobClient.DownloadAsync();

            return blobDownloadInfo.Value;
        }
        /// <summary>
        /// Get Blob Url
        /// </summary>
        /// <param name="containerName">Name container</param>
        /// <param name="fileName">File Name</param>
        /// <returns></returns>
        public string GetBlobUrlAsync(string containerName, string fileName)
        {
            var containerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            var expiration = DateTimeOffset.Now.AddHours(2);

            var uri = blobClient.GenerateSasUri(BlobSasPermissions.Read, expiration);

            return uri.ToString();
        }
        /// <summary>
        /// Upload Blob IformFile
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <param name="file">File Base64</param>
        /// <param name="containerName">name container</param>
        /// <returns></returns>
        public async Task<dynamic> UploadBlobAsync(string fileName, IFormFile file, string containerName)
        {
            //var fileName = $"{Guid.NewGuid()}.{fileExtension}";
            var containerClient = _blobClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            containerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var res = await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });
            return res; //!= null;
        }
        /// <summary>
        /// Upload Blob Stream File
        /// </summary>
        /// <param name="fileExtension">Extension</param>
        /// <param name="uploadFileStream">Stream</param>
        /// <param name="containerName">Name Container</param>
        /// <param name="overwrite">Override File </param>
        /// <returns></returns>
        public async Task<dynamic> UploadBlobAsync(string fileName, Stream uploadFileStream, string containerName, bool overwrite)
        {
            //var fileName = $"{Guid.NewGuid()}.{fileExtension}";
            var containerClient = _blobClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            containerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var res = await blobClient.UploadAsync(uploadFileStream, overwrite);            
            return res;//!= null;
        }
        /// <summary>
        /// List Files
        /// </summary>
        /// <returns></returns>
        public async Task<List<BlobItem>> ListAllItemsInContainer()
        {
            var listBlobs = new List<BlobItem>();
            await foreach (var blobContaineritem in _blobClient.GetBlobContainersAsync())
            {
                var blobContainerClient = _blobClient.GetBlobContainerClient(blobContaineritem.Name);

                await foreach (var blobItem in blobContainerClient.GetBlobsAsync())
                {
                    listBlobs.Add(blobItem);
                }
            }
            return listBlobs;
        }
        /// <summary>
        /// Delete Blob
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="containerName">Container Name</param>
        /// <returns></returns>
        public async Task<bool> DeleteBlob(string name, string containerName)
        {
            var containerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(name);
            return await blobClient.DeleteIfExistsAsync();
        }
    }
}
