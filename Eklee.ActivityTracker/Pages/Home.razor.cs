using Eklee.ActivityTracker.Models;
using Eklee.ActivityTracker.Services;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Eklee.ActivityTracker.Pages;

public partial class Home
{
    [Inject] ActivityService? ActivityService { get; set; }
    [Inject] DialogService? DialogService { get; set; }

    private List<HomeActivity>? activities;

    protected override async Task OnInitializedAsync()
    {
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        activities = (await ActivityService!.GetActivitiesAsync()).Select(x => new HomeActivity(x)).ToList();
    }

    private void SelectActivity(HomeActivity activity)
    {
        activity.StartTimerView = true;
        
    }

    private async Task NewActivity()
    {
        var activity = new Activity();
        bool? result = await DialogService!.OpenAsync<EditActivity>("Activity",
            new Dictionary<string, object>() {
                { "Model", activity },
                { "IsNew", true },
                { "DialogService", DialogService } },
            new DialogOptions() { Width = "500px", Height = "300px", Resizable = true, Draggable = true });

        if (result == true)
        {
            await ActivityService!.SaveActivityAsync(activity);
            await RefreshAsync();
        }
    }

    private async Task DeleteActivity(HomeActivity activity)
    {
        await ActivityService!.DeleteActivityAsync(activity.GetActivity());
        await RefreshAsync();
    }
}
