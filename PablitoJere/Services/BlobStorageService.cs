using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
            connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")!;
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
    }
}
