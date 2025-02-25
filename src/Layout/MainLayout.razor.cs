using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Eklee.ActivityTracker.Layout;

public partial class MainLayout
{
    [Inject] NavigationManager Navigation { get; set; } = default!;

    private void SignOut()
    {
        Navigation.NavigateToLogout("authentication/logout");
    }

    private void SignIn()
    {
        Navigation.NavigateToLogin("/");
    }
}
