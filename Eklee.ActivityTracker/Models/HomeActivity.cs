namespace Eklee.ActivityTracker.Models;

public class HomeActivity(Activity activity)
{
    public string Name => activity.Name;

    //public bool StartTimerView { get; set; }

    private int GetSessionCount()
    {
        if (activity.Sessions is null || activity.Sessions.Length == 0)
        {
            return 0;
        }
        return activity.Sessions.Length;
    }

    private double GetAvgDurationInSeconds()
    {
        if (activity.Sessions is null || activity.Sessions.Length == 0)
        {
            return 0;
        }
        return activity.Sessions.Average((ActivitySession x) => x.Items!.Average((ActivityItem y) => y.DurationInSeconds!.Value));
    }

    public List<HomeActivityStat> GetStats()
    {
        return
        [
            new HomeActivityStat("Session Count", GetSessionCount().ToString()),
            new HomeActivityStat("Avg Duration", double.Round( GetAvgDurationInSeconds(),2).ToString())
        ];
    }

    public Activity GetActivity()
    {
        return activity;
    }
}

public record HomeActivityStat(string Key, string Value);
