using Azure.Storage.Blobs;

public class AzureStorageService
{
    private readonly string _connectionString;

    public AzureStorageService(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");
        }

        _connectionString = connectionString;
    }

    public string GetBlobUrl(string containerName, string blobName)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        return blobClient.Uri.ToString();
    }
}
