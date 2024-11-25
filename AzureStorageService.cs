using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using System.IO;
using System.Threading.Tasks;

public class AzureStorageService
{
    private readonly string _connectionString;

public AzureStorageService(IConfiguration configuration)
{
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
        // Download a file from Azure Blob Storage
    public async Task<Stream> DownloadFileAsync(string containerName, string blobName)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        // Create a MemoryStream to store the downloaded content
        var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream);

        // Reset the position of the MemoryStream before returning it
        memoryStream.Position = 0;

        return memoryStream;
    }

    // Upload a file to Azure Blob Storage
    public async Task UploadFileAsync(string containerName, string blobName, Stream fileStream)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        // Upload the file stream to the blob
        await blobClient.UploadAsync(fileStream, overwrite: true);
    }

public string GenerateBlobSasToken(string blobUrl, string storageAccountName, string storageAccountKey)
{
    // Create a BlobServiceClient
    var blobServiceClient = new BlobServiceClient(new Uri($"https://{storageAccountName}.blob.core.windows.net"), 
        new StorageSharedKeyCredential(storageAccountName, storageAccountKey));

    // Get a reference to the blob
    BlobClient blobClient = blobServiceClient.GetBlobContainerClient("background-images").GetBlobClient("background.jpg");

    // Define SAS token permissions and expiry
    BlobSasBuilder sasBuilder = new BlobSasBuilder
    {
        BlobContainerName = "background-images",
        BlobName = "background.jpg",
        Resource = "b", // "b" for blob, "c" for container
        StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1) // Token validity
    };

    sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

    // Generate the SAS token
    string sasToken = sasBuilder.ToSasQueryParameters(
        new StorageSharedKeyCredential(storageAccountName, storageAccountKey))
        .ToString();

    // Combine the blob URL with the SAS token
    return $"{blobClient.Uri}?{sasToken}";
}
}