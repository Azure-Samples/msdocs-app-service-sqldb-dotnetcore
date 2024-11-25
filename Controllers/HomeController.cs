using System.Diagnostics;
using DotNetCoreSqlDb.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreSqlDb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AzureStorageService _azureStorageService;

        public HomeController(ILogger<HomeController> logger, AzureStorageService azureStorageService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public IActionResult Index()
        {
            string storageAccountName = "acewwwroot";
            string storageAccountKey = "rANrk8t68qtk5ooKS4T+gRHGYHGdpFZRGUEz+iEWdVL5l3dwGgo8tAtSsxWPTGLBYqg9/Iz1g3L/+AStRxhgRw=="; // Get this securely from configuration
            string blobUrl = "https://acewwwroot.blob.core.windows.net/background-images/background.jpg";

    // Generate SAS token URL
            string sasUrl = _azureStorageService.GenerateBlobSasToken(blobUrl, storageAccountName, storageAccountKey);

            ViewData["BackgroundUrl"] = sasUrl;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Using Activity to get the request ID
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
