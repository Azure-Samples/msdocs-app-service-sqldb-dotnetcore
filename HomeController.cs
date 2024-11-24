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

        public IActionResult Index([FromServices] AzureStorageService azureStorageService)
        {
            var backgroundUrl = azureStorageService.GetBlobUrl("background-images", "background.jpg");
            ViewData["BackgroundUrl"] = backgroundUrl;
            return View();
        }
    }

    // Service to handle Azure Blob Storage interactions
public class AzureStorageService
    {
    private readonly string connectionString;

    public AzureStorageService(IConfiguration configuration)
    {
        // Ensure _connectionString is initialized
        connectionString = configuration.GetConnectionString("AzureStorage")
            ?? throw new ArgumentNullException(nameof(connectionString), "Azure Storage connection string is not configured.");
    }

    public string GetBlobUrl(string containerName, string blobName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        return blobClient.Uri.ToString();
    }
    }
}