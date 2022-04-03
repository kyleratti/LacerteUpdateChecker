namespace ApiPoller.Config;

// ReSharper disable once ClassNeverInstantiated.Global
public class NotificationSettings
{
	private readonly IConfiguration _config;

	public NotificationSettings(IConfiguration config)
	{
		_config = config;
	}

	public string[] RecipientEmailAddresses => Settings.RecipientEmailAddresses;

	private RawNotificationSettings Settings
	{
		get
		{
			var obj = new RawNotificationSettings();
			_config.GetSection(nameof(NotificationSettings)).Bind(obj);
			return obj;
		}
	}

	private class RawNotificationSettings
	{
		public string[] RecipientEmailAddresses { get; set; } = null!;
	}
}