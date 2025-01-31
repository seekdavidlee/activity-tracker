﻿namespace Eklee.ActivityTracker.Models;

public class Activity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ActivityItem[]? Items { get; set; }

    public bool? Archive { get; set; }
}
