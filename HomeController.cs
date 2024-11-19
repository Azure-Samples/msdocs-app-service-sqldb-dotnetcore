using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string imageUrl = GetBlobUrl("images", "background.jpg"); // Retrieve URL of the image
            ViewData["BackgroundImageUrl"] = imageUrl; // Pass it to the view
            return View(); // Return the view to be rendered
        }

        // Method to get the blob URL (assuming you have it in this controller or as a service)
        private string GetBlobUrl(string containerName, string blobName)
        {
            string connectionString = "YourAzureStorageConnectionString"; // Replace with your actual connection string
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            return blobClient.Uri.ToString();
        }
    }
}
