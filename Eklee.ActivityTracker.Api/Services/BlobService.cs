using Azure.Identity;
using Azure.Storage.Blobs;

namespace Eklee.ActivityTracker.Api.Services;

public class BlobService(Config config)
{
    private readonly BlobServiceClient blobServiceClient = new(config.StorageUri, new DefaultAzureCredential());

    private BlobContainerClient? blobContainerClient = null;
    private BlobContainerClient BlobContainerClient
    {
        get
        {
            blobContainerClient ??= blobServiceClient.GetBlobContainerClient(config.StorageContainerName);
            return blobContainerClient;
        }
    }

    public Task SaveAsync(string userPrefix, string key, string value)
    {
        return BlobContainerClient.GetBlobClient($"{userPrefix}/{key}").UploadAsync(BinaryData.FromString(value), overwrite: true);
    }

    public Task DeleteAsync(string userPrefix, string key)
    {
        return BlobContainerClient.GetBlobClient($"{userPrefix}/{key}").DeleteIfExistsAsync();
    }

    public async Task<IEnumerable<T>> ListAsync<T>(string userPrefix)
    {
        List<T> items = [];
        await foreach (var b in BlobContainerClient.GetBlobsAsync(prefix: userPrefix))
        {
            var c = BlobContainerClient.GetBlobClient(b.Name);
            var res = await c.DownloadContentAsync();
            items.Add(res.Value.Content.ToObjectFromJson<T>()!);
        }
        return items;
    }
}
