namespace NamePairing;

public interface IBrandProvider
{
    public string AppName { get; }
    public string GreetingPrefix { get; }
    public string AppHeader { get; }
    public string MatchString { get; }
}

public class DefaultBrandProvider : IBrandProvider
{
    public string AppName => "Name Pairing";
    public string GreetingPrefix => "Hi";
    public string AppHeader => string.Empty;
    public string MatchString => "You are matched with";

}

public class ConfigBrandProvider : IBrandProvider
{
    public ConfigBrandProvider(IConfigurationSection section) {
        var defaultBrand = new DefaultBrandProvider();
        if (section.Exists()) {
            AppName = section.GetValue<string>("AppName", defaultBrand.AppName);
            GreetingPrefix = section.GetValue<string>("GreetingPrefix", defaultBrand.GreetingPrefix);
            AppHeader = section.GetValue("AppHeader", defaultBrand.AppHeader);
            MatchString = section.GetValue(nameof(MatchString), defaultBrand.MatchString);
        }
    }

    public string AppName { get; } = string.Empty;
    public string GreetingPrefix { get; } = string.Empty;
    public string AppHeader { get; } = string.Empty;
    public string MatchString { get; } = string.Empty;
}