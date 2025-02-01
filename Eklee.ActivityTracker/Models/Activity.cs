namespace Eklee.ActivityTracker.Models;

public class Activity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ActivitySession[]? Sessions { get; set; }
    public bool? Archive { get; set; }
    public DateTime? LastUpdated { get; set; }
}

public class ActivitySession
{
    public DateTime? Start { get; set; }
    public ActivityItem[]? Items { get; set; }
}
