using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PablitoJere.Entities;
using PablitoJere.Utilities;
using System.Drawing;

namespace PablitoJere.Services
{
    public class BlobStorageService
    {
        string connectionString;
        string containerName;
        BlobServiceClient blobServiceClient;
        public BlobStorageService()
        {
            connectionString = "DefaultEndpointsProtocol=https;AccountName=pablitojere;AccountKey=Qj1NQzNvRLVqYj5mR/dRIzWcXuhD+Do3FSW1/rt+dhzCi8OR9TnhFPsK8FdWRtmHsnWHDq7Pvy78+ASt5hGvmQ==;EndpointSuffix=core.windows.net";
            blobServiceClient = new BlobServiceClient(connectionString);
            containerName = "publications";
        }
        public async Task<string> UploadImageToBlob(string fileName, string base64Image) {

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            DataImage dataImage = DataImage.TryParse(base64Image);

            using (var stream = new MemoryStream(dataImage.RawData))
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = dataImage.MimeType });
            }

            return "https://pablitojere.blob.core.windows.net/" + containerName + "/" + fileName;
        }

        public async Task<string[]> UploadImagesToBlobStorage(List<string> publicationImages)
        {
            List<Task<string>> blobTasks = new List<Task<string>>();

            publicationImages.ForEach(base64Image => blobTasks.Add(
                this.UploadImageToBlob(Guid.NewGuid().ToString(), base64Image)));

            return await Task.WhenAll(blobTasks);
        }

        public async Task<bool> DeleteFileFromContainer(string identifier)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            bool value = await containerClient.DeleteBlobIfExistsAsync(identifier);
            return value;
        }

        public async Task<bool[]> DeleteFilesFromContainer(List<string> identifiers)
        {
            List<Task<bool>> blobTasks = new List<Task<bool>>();
            
            identifiers.ForEach(identifier => blobTasks.Add(DeleteFileFromContainer(identifier)));

            return await Task.WhenAll(blobTasks);
        }

        internal List<string> GetIdentifiers(List<PublicationImage> publicationImages)
        {
            List<string> unformattedIdentifiers = publicationImages.Select(x => x.ImageUrl).ToList();
            List<string> identifiers = new List<string>();
            unformattedIdentifiers.ForEach(unformattedIdentifier => identifiers.Add(unformattedIdentifier.Substring(unformattedIdentifier.LastIndexOf('/') + 1)));

            return identifiers;
        }
    }
}
