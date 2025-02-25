using Eklee.ActivityTracker.Models;
using Eklee.ActivityTracker.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Eklee.ActivityTracker.Pages;

public partial class Home
{
    [Inject] ActivityService? ActivityService { get; set; }
    [Inject] DialogService? DialogService { get; set; }

    private readonly List<HomeActivity> activities = [];
    private ActivitySession? activitySession;
    private HomeActivity? activeActivity;
    private RadzenDataList<HomeActivity>? radzenDataList;
    public bool SelectMode { get; set; }
    private DateTime Current { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        activities.Clear();
        activities.AddRange([.. (await ActivityService!.GetActivitiesAsync()).Select(x => new HomeActivity(x)).OrderByDescending(x => x.GetActivity().LastUpdated)]);
        await radzenDataList!.FirstPage();
    }

    private void SelectActivity(HomeActivity activity)
    {
        Current = DateTime.Now;
        if (activitySession is null)
        {
            activitySession = new ActivitySession
            {
                Start = Current
            };

            activitySession.Items ??= [];
        }

        var item = new ActivityItem
        {
            Start = Current,
        };
        activitySession.Items = [.. activitySession.Items, item];
        activeActivity = activity;
    }

    private void RestartActivity()
    {
        if (activitySession is null || activeActivity is null)
        {
            return;
        }

        activitySession.Items![^1].Start = DateTime.Now;
        Current = activitySession.Items![^1].Start!.Value;
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


        activitySession = null;
        activeActivity = null;
        await RefreshAsync();
    }

    private async Task CancelActivity()
    {
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

        Current = DateTime.Now;
        activitySession.Items = [.. activitySession.Items, new ActivityItem
        {
            Start = Current
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

    private async Task DeleteActivity()
    {
        var toDelete = activities.Where(x => x.Selected);
        if (!toDelete.Any())
        {
            return;
        }

        foreach (var activity in toDelete)
        {
            await ActivityService!.DeleteActivityAsync(activity.GetActivity());
        }
        await RefreshAsync();
    }

    private void EnableSelectMode()
    {
        SelectMode = true;
    }

    private void DisableSelectMode()
    {
        foreach (var activity in activities.Where(x => x.Selected))
        {
            activity.Selected = false;
        }
        SelectMode = false;
    }
}
