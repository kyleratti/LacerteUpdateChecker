namespace ApiPoller.Config;

public class LacerteApiSettings
{
	private readonly IConfiguration _config;

	public LacerteApiSettings(IConfiguration config)
	{
		_config = config;
	}

	public string ApiUrl => Settings.ApiUrl;
	public string ApiNamespace => Settings.ApiNamespace;
	public string ApplicationStatePath => Settings.ApplicationStatePath;

	private ApiSettings Settings
	{
		get
		{
			var obj = new ApiSettings();
			_config.GetSection("LacerteApiSettings").Bind(obj);
			return obj;
		}
	}

	private class ApiSettings
	{
		public string ApiUrl { get; set; } = null!;
		public string ApiNamespace { get; set; } = null!;
		public string ApplicationStatePath { get; set; } = null!;
	}
}