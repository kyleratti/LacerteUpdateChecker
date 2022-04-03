namespace ApiPoller.Config;

public class GetUpdateManifestActionSettings
{
	private readonly IConfiguration _config;

	public GetUpdateManifestActionSettings(IConfiguration config)
	{
		_config = config;
	}

	public string ActionName => Settings.ActionName;
	public string ProductName => Settings.ProductName;
	public int ProductVersion => Settings.ProductVersion;
	public int MinorVersion => Settings.MinorVersion;
	public int IncrementalVersion => Settings.IncrementalVersion;
	public bool ExcludeLocations => Settings.ExcludeLocations;
	public string SourceName => Settings.SourceName;

	private GetUpdateManifestAction Settings
	{
		get
		{
			var obj = new GetUpdateManifestAction();
			_config.GetSection("GetUpdateManifestAction").Bind(obj);
			return obj;
		}
	}
}

public class GetUpdateManifestAction
{
	public string ActionName { get; set; }
	public string ProductName { get; set; }
	public int ProductVersion { get; set; }
	public int MinorVersion { get; set; }
	public int IncrementalVersion { get; set; }
	public bool ExcludeLocations { get; set; }
	public string SourceName { get; set; }
}