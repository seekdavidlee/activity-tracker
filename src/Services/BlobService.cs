using System.Net.Http.Json;

namespace Eklee.ActivityTracker.Services;

public class BlobService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient httpClient = httpClientFactory.CreateClient(nameof(BlobService));
    private const string urlPrefix = "filesystemapi/files/object?path=prod/activity-tracker/";

    public Task SaveAsync(string userPrefix, string key, string value)
    {
        return httpClient.PutAsync($"{urlPrefix}{userPrefix}/{key}", new StringContent(value));
    }

    public Task DeleteAsync(string userPrefix, string key)
    {
        return httpClient.DeleteAsync($"filesystemapi/files?path=prod/activity-tracker/{userPrefix}/{key}");
    }

    public async Task<IEnumerable<T>> ListAsync<T>(string userPrefix)
    {
        string resources = $"filesystemapi/files?path=prod/activity-tracker/{userPrefix}/";
        var response = await httpClient.GetFromJsonAsync<string[]>(resources);

        if (response != null)
        {
            List<T> result = [];
            foreach (var item in response)
            {
                if (item is null) continue;

                var itemResponse = await httpClient.GetFromJsonAsync<T>($"filesystemapi/files/object?path={item}");
                if (itemResponse is null) continue;

                result.Add(itemResponse);
            }
            return result;
        }

        return [];
    }
}
