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
            var backgroundUrl = _azureStorageService.GetBlobUrl("background-images", "background.jpg");
            ViewData["BackgroundUrl"] = "https://acewwwroot.blob.core.windows.net/background-images/background.jpg?sv=2022-11-02&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2025-06-30T22:51:04Z&st=2024-11-17T15:51:04Z&spr=https&sig=i0YOLy6F46uqF4dO0plDdw8V590gGhumaudcZiFjrg8%3D";

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
