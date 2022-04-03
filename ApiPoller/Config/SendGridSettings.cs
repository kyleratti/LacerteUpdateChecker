namespace ApiPoller.Config;

public class SendGridSettings
{
	private readonly IConfiguration _config;

	public SendGridSettings(IConfiguration config)
	{
		_config = config;
	}

	public string ApiKey => Settings.ApiKey;
	public string FromAddress => Settings.FromAddress;
	public string FromName => Settings.FromName;
	public string Subject => Settings.Subject;

	private RawSendGridSettings Settings
	{
		get
		{
			var obj = new RawSendGridSettings();
			_config.GetSection(nameof(SendGridSettings)).Bind(obj);
			return obj;
		}
	}

	private class RawSendGridSettings
	{
		public string ApiKey { get; set; } = null!;
		public string FromAddress { get; set; } = null!;
		public string FromName { get; set; } = null!;
		public string Subject { get; set; } = null!;
	}
}