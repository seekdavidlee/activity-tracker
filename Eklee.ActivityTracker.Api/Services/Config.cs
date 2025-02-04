namespace Eklee.ActivityTracker.Api.Services;

public class Config
{
    public Config()
    {
        StorageUri = new Uri(Environment.GetEnvironmentVariable(nameof(StorageUri)) ?? throw new Exception("StorageUri is not configured"));
        StorageContainerName = Environment.GetEnvironmentVariable(nameof(StorageContainerName)) ?? "";
    }
    public Uri StorageUri { get; }

    public string StorageContainerName { get; }
}
