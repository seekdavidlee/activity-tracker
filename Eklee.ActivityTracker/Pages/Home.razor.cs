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
    private ActivitySession? activitySession;
    private HomeActivity? activeActivity;

    protected override async Task OnInitializedAsync()
    {
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        activities = [.. (await ActivityService!.GetActivitiesAsync()).Select(x => new HomeActivity(x)).OrderByDescending(x => x.GetActivity().LastUpdated)];
    }

    private void SelectActivity(HomeActivity activity)
    {
        activity.StartTimerView = true;
        if (activitySession is null)
        {
            activitySession = new ActivitySession
            {
                Start = DateTime.Now
            };

            activitySession.Items ??= [];
        }

        var item = new ActivityItem
        {
            Start = DateTime.Now
        };
        activitySession.Items = [.. activitySession.Items, item];
        activeActivity = activity;
    }

    private async Task StopActivityAsync()
    {
        if (activitySession is null || activeActivity is null)
        {
            return;
        }

        activitySession.Items![^1].DurationInSeconds = Convert.ToInt32((DateTime.Now - activitySession.Items![^1].Start!.Value).TotalSeconds);
        if (activeActivity!.GetActivity().Sessions is null)
        {
            activeActivity.GetActivity().Sessions = [activitySession];
        }
        else
        {
            activeActivity.GetActivity().Sessions = [.. activeActivity.GetActivity().Sessions, activitySession];
        }

        await ActivityService!.SaveActivityAsync(activeActivity.GetActivity());

        activeActivity.StartTimerView = false;
        activitySession = null;
        activeActivity = null;
        await RefreshAsync();
    }

    private void NextActivity()
    {
        if (activitySession is null)
        {
            return;
        }

        activitySession.Items![^1].DurationInSeconds = Convert.ToInt32((DateTime.Now - activitySession.Items![^1].Start!.Value).TotalSeconds);

        activitySession.Items = [.. activitySession.Items, new ActivityItem
        {
            Start = DateTime.Now
        }];
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
