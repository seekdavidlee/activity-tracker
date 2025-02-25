using Microsoft.JSInterop;

namespace Eklee.ActivityTracker.Services;

public class LocalStorageHelper(IJSRuntime js)
{
    public async Task SetItemAsync(string key, string value)
    {
        await js.InvokeVoidAsync("localStorageHelper.setItem", key, value);
    }

    public async Task<string> GetItemAsync(string key)
    {
        return await js.InvokeAsync<string>("localStorageHelper.getItem", key);
    }

    public async Task RemoveItemAsync(string key)
    {
        await js.InvokeVoidAsync("localStorageHelper.removeItem", key);
    }

    public async Task ClearAsync()
    {
        await js.InvokeVoidAsync("localStorageHelper.clear");
    }
}