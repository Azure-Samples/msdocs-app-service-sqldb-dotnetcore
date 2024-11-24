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
    private readonly ILogger<AzureStorageService> _logger;

    public AzureStorageService(IConfiguration configuration, ILogger<AzureStorageService> logger)
    {
        _connectionString = configuration.GetConnectionString("AzureStorage")
            ?? throw new ArgumentNullException(nameof(_connectionString), "Azure Storage connection string is not configured.");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

public string GetBlobUrl(string containerName, string blobName)
{
    try
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        return blobClient.Uri.ToString();
    }
    catch (Exception ex)
    {
        // Log the error for troubleshooting
        _logger.LogError($"Error retrieving blob URL: {ex.Message}");
        throw new InvalidOperationException("Error retrieving blob URL.", ex);
    }
}

    }
}
