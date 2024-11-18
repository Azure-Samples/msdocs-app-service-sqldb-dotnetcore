using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;

public class FileController : Controller
{
    private readonly AzureStorageService _azureStorageService;

    // Inject the AzureStorageService into the controller through constructor dependency injection
    public FileController(AzureStorageService azureStorageService)
    {
        _azureStorageService = azureStorageService;
    }

    // An example action method to download a file from Azure Blob Storage
    public async Task<IActionResult> Download(string containerName, string blobName)
    {
        // Call the AzureStorageService to download the file
        var fileStream = await _azureStorageService.DownloadFileAsync(containerName, blobName);

        // Return the file to the client as a downloadable stream
        return File(fileStream, "application/octet-stream", blobName);
    }

    // An example action method to upload a file to Azure Blob Storage
    [HttpPost]
    public async Task<IActionResult> Upload(string containerName, string blobName)
    {
        // Get the uploaded file from the request (assuming a form post with a file)
        using var fileStream = Request.Form.Files[0].OpenReadStream();

        // Call the AzureStorageService to upload the file
        await _azureStorageService.UploadFileAsync(containerName, blobName, fileStream);

        return Ok("File uploaded successfully");
    }
}
