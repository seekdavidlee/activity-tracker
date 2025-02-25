
using Eklee.ActivityTracker.Models;
using System.Text.Json;

namespace Eklee.ActivityTracker.Services;

public class ActivityService(BlobService blobService, LocalStorageHelper localStorageHelper)
{
    private async Task<string> GetUsername()
    {
        var username = await localStorageHelper.GetItemAsync(nameof(GetUsername));
        if (string.IsNullOrEmpty(username))
        {
            username = Guid.NewGuid().ToString("N");
            await localStorageHelper.SetItemAsync(nameof(GetUsername), username);
        }

        return username;
    }

    public async Task<List<Activity>> GetActivitiesAsync()
    {
        var results = await blobService.ListAsync<Activity>(await GetUsername());
        return results.ToList();
    }

    public async Task SaveActivityAsync(Activity activity)
    {
        if (string.IsNullOrEmpty(activity.Id))
        {
            activity.Id = $"{Guid.NewGuid():N}.json";
        }
        activity.LastUpdated = DateTime.Now;

        await blobService.SaveAsync(await GetUsername(), activity.Id, JsonSerializer.Serialize(activity));
    }

    public async Task DeleteActivityAsync(Activity activity)
    {
        await blobService.DeleteAsync(await GetUsername(), activity.Id);
    }
}
