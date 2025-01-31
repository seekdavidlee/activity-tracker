namespace Eklee.ActivityTracker.Models;

public class HomeActivity(Activity activity)
{
    public string Name => activity.Name;

    public bool StartTimerView { get; set; }

    public string DisplayStats
    {
        get
        {
            if (activity.Items is null || activity.Items.Length == 0)
            {
                return "No items, click to select";
            }
            return $"Total records: {activity.Items.Length}";
        }
    }

    public Activity GetActivity()
    {
        return activity;
    }
}
