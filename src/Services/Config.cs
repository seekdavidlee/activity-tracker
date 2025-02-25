using System.Security.Claims;
using System.Text.Json;

namespace Eklee.ActivityTracker.Services;

public class Config
{
    private readonly string header;
    public Config(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var config = configuration.GetSection("System");
        header = config["Header"] ?? "Please configure text for header!";
        Footer = config["Footer"] ?? "Please configure text for footer!";

        StorageUri = new Uri(configuration[nameof(StorageUri)]!);
    }

    public Uri StorageUri { get; }

    public string Header
    {
        get
        {
            return $"{header} - {Username}";
        }
    }

    public string Footer { get; }

    public string Username { get; set; } = Constants.Unknown;

    public string Displayname { get; set; } = Constants.Unknown;
}
