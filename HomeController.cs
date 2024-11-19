using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    public class HomeController : Controller
    {
        private readonly AzureStorageService _azureStorageService;

        // Constructor with dependency injection of the AzureStorageService
        public HomeController(AzureStorageService azureStorageService)
        {
            _azureStorageService = azureStorageService;
        }

        public async Task<IActionResult> Index()
        {
            // Retrieve the URL of the background image
            string imageUrl = await _azureStorageService.GetBlobUrlAsync("images", "background.jpg");
            ViewData["BackgroundImageUrl"] = imageUrl; // Pass it to the view
            return View(); // Return the view to be rendered
        }
    }

    // Service to handle Azure Blob Storage interactions
    public class AzureStorageService
    {
        private readonly string _connectionString;

        // Constructor to inject the Azure Storage connection string
        public AzureStorageService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Method to get the URL of a blob (file) in a container
        public async Task<string> GetBlobUrlAsync(string containerName, string blobName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            return blobClient.Uri.ToString(); // Return the URI of the blob
        }

        // Method to download a file (blob) from a specified container
        public async Task<Stream> DownloadFileAsync(string containerName, string blobName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            var response = await blobClient.DownloadStreamingAsync();
            return response.Value.Content; // Return the file content as a stream
        }

        // Method to upload a file (blob) to a specified container
        public async Task UploadFileAsync(string containerName, string blobName, Stream content)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(content, overwrite: true); // Upload the file and overwrite if it exists
        }
    }
}
