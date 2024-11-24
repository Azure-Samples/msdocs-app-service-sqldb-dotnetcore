using DotNetCoreSqlDb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Azure.Storage.Blobs;

namespace DotNetCoreSqlDb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AzureStorageService _azureStorageService;

        // Constructor with dependency injection of the AzureStorageService
        public HomeController(ILogger<HomeController> logger, AzureStorageService azureStorageService)
        {
            _logger = logger;
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public IActionResult Index()
        {
            // Use the injected AzureStorageService
            var backgroundUrl = _azureStorageService.GetBlobUrl("background-images", "background.jpg");
            ViewData["BackgroundUrl"] = backgroundUrl;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    // Service to handle Azure Blob Storage interactions
    public class AzureStorageService
    {
        private readonly string _connectionString;

        public AzureStorageService(IConfiguration configuration)
        {
            // Ensure _connectionString is initialized
            _connectionString = configuration.GetConnectionString("AzureStorage")
                ?? throw new ArgumentNullException(nameof(_connectionString), "Azure Storage connection string is not configured.");
        }

        public string GetBlobUrl(string containerName, string blobName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            return blobClient.Uri.ToString();
        }
    }
}
