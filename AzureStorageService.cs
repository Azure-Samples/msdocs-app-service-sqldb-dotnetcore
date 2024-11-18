using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;

public class AzureStorageService
{
    private readonly string _connectionString;

    // Constructor that accepts a connection string
    public AzureStorageService(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Method to download a file (blob) from a specified container
    public async Task<Stream> DownloadFileAsync(string containerName, string blobName)
    {
        // Create a Blob service client using the connection string
        var blobServiceClient = new BlobServiceClient(_connectionString);

        // Get a reference to the container
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // Get a reference to the blob (file) you want to download
        var blobClient = containerClient.GetBlobClient(blobName);

        // Download the blob and return the stream
        var response = await blobClient.DownloadStreamingAsync();
        return response.Value.Content;
    }

    // Method to upload a file (blob) to a specified container
    public async Task UploadFileAsync(string containerName, string blobName, Stream content)
    {
        // Create a Blob service client using the connection string
        var blobServiceClient = new BlobServiceClient(_connectionString);

        // Get a reference to the container (create it if it doesn't exist)
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        // Get a reference to the blob (file) you want to upload
        var blobClient = containerClient.GetBlobClient(blobName);

        // Upload the file content
        await blobClient.UploadAsync(content, overwrite: true);
    }
}
