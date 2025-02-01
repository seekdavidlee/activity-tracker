using Microsoft.AspNetCore.Components.Authorization;

namespace Eklee.ActivityTracker.Services;

public class UserService(AuthenticationStateProvider authenticationStateProvider)
{
    public async Task<string> GetUserNameAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            return user.Identity.Name!.Replace("@", "_").Replace(".", "_").Replace(" ", "");
        }

        return "anonymous";
    }
}
