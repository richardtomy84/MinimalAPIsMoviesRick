
using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace MinimalAPIsMoviesRick.Services
{
    public class AzureFileStorage(IConfiguration configuration) : IFileStorage
    {
        private string connectionString = configuration.GetConnectionString("AzureStorage")!;

        public async  Task Delete(string? route, string container)
        {
            if (string.IsNullOrEmpty(route))
            {
                return;
            }
            var client = new BlobContainerClient(connectionString, container);
            await client.CreateIfNotExistsAsync();
            var fileName = Path.GetFileName(route);
            var blob = client.GetBlobClient(fileName);
            await blob.DeleteIfExistsAsync();
 
            //throw new NotImplementedException();
        }

        public async Task<string> Store(string container, IFormFile file)
        {
            var client = new BlobContainerClient(connectionString, container);
            await client.CreateIfNotExistsAsync();
            client.SetAccessPolicy(PublicAccessType.Blob);

            var extention = Path.GetExtension(file.Name);
            var fileName = $"{Guid.NewGuid()}{extention}";
            var blob = client.GetBlobClient(fileName);
            BlobHttpHeaders blobHttpHeaders = new();
            blobHttpHeaders.ContentType = file.ContentType;
            await blob.UploadAsync(file.OpenReadStream(), blobHttpHeaders);
            return blob.Uri.ToString();
           // throw new NotImplementedException();
        }
    }
}

