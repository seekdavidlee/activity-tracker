using Eklee.ActivityTracker.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Eklee.ActivityTracker.Pages;

public partial class EditActivity
{
    [Parameter] public Activity? Model { get; set; }

    [Parameter]
    public DialogService? DialogService { get; set; }

    [Parameter]
    public bool IsNew { get; set; }

    string? submitIcon;
    string? submitTitle;
    string? cancelTitle;

    protected override void OnInitialized()
    {
        submitIcon = IsNew ? "add" : "save";
        submitTitle = IsNew ? "Add Activity" : "Save Activity";
        cancelTitle = IsNew ? "Cancel add Activity" : "Cancel update Activity";
    }

    void Submit(Activity activity)
    {
        DialogService?.Close(true);
    }

    void Cancel()
    {
        DialogService?.Close(false);
    }
}
