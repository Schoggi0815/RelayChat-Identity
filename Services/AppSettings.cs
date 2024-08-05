namespace RelayChat_Identity.Services;

public class AppSettings
{
    private readonly IConfiguration _config;

    public AppSettings(IConfiguration config)
    {
        _config = config;
        JWT = new JwtAppSettings
        {
            Secret = GetSetting<string>(nameof(JWT), nameof(JwtAppSettings.Secret))
        };
    }

    private T GetSetting<T>(string sectionKey, string valueKey)
    {
        return _config.GetSection(sectionKey).GetValue<T>(valueKey) ??
               throw new Exception($"config \"{sectionKey}:{valueKey}\" of type \"{typeof(T)}\" not found!");
    }

    public JwtAppSettings JWT { get; set; }
}

public class JwtAppSettings
{
    public required string Secret { get; set; }
}