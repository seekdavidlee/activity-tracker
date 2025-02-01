
using Eklee.ActivityTracker.Models;
using System.Text.Json;

namespace Eklee.ActivityTracker.Services;

public class ActivityService(BlobService blobService, UserService userService)
{
    public async Task<List<Activity>> GetActivitiesAsync()
    {
        var username = await userService.GetUserNameAsync();
        var results = await blobService.ListAsync<Activity>(username);
        return results.ToList();
    }

    public async Task SaveActivityAsync(Activity activity)
    {
        if (string.IsNullOrEmpty(activity.Id))
        {
            activity.Id = $"{Guid.NewGuid():N}.json";
        }
        activity.LastUpdated = DateTime.Now;
        var username = await userService.GetUserNameAsync();
        await blobService.SaveAsync(username, activity.Id, JsonSerializer.Serialize(activity));
    }

    public async Task DeleteActivityAsync(Activity activity)
    {
        var username = await userService.GetUserNameAsync();
        await blobService.DeleteAsync(username, activity.Id);
    }
}
