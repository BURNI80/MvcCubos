using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using MvcCubos.Models;
using Azure.Storage.Sas;

namespace MvcCubos.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }


        public async Task<List<BlobModel>> GetBlobsAsync
            (string containerName)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            List<BlobModel> blobModels = new List<BlobModel>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                BlobClient blobClient =
                    containerClient.GetBlobClient(item.Name);
                BlobModel model = new BlobModel();
                model.Nombre = item.Name;
                model.Contenedor = containerName;
                model.Url = blobClient.Uri.AbsoluteUri;
                blobModels.Add(model);
            }
            return blobModels;
        }

        public async Task DeleteBlobAsync
            (string containerName, string blobName)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.DeleteBlobAsync(blobName);
        }

        public async Task UploadBlobAsync
            (string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }

        public async Task<string> GetUrl(string container, string name)
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(container);
            BlobContainerProperties prop = containerClient.GetProperties();
            BlobClient blobClient = containerClient.GetBlobClient(name);
            if (prop.PublicAccess == PublicAccessType.None)
            {
                Uri imageUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddSeconds(3600));
                return imageUri.ToString();
            }
            else
            {
                return blobClient.Uri.AbsoluteUri;
            }
        }
    }

}
