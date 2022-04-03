using ApiPoller.Config;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ApiPoller.Services;

public class Emailer
{
	private readonly SendGridSettings _sendGridSettings;
	private readonly NotificationSettings _notificationSettings;

	public Emailer(SendGridSettings sendGridSettings, NotificationSettings notificationSettings)
	{
		_sendGridSettings = sendGridSettings;
		_notificationSettings = notificationSettings;
	}

	public async Task SendNewVersionNotification(DateTime detectedAt, DateTime updatedAt, DateTime nextCheckAt, CancellationToken cancellationToken)
	{
		var client = new SendGridClient(_sendGridSettings.ApiKey);
		var msg = new SendGridMessage
		{
			From = new EmailAddress(_sendGridSettings.FromAddress, _sendGridSettings.FromName),
			Subject = string.Format(_sendGridSettings.Subject),
			HtmlContent = GetMessageBody()
		};
		msg.AddTos(_notificationSettings.RecipientEmailAddresses.Select(x => new EmailAddress(x)).ToList());

		var resp = await client.SendEmailAsync(msg, cancellationToken);

		if (!resp.IsSuccessStatusCode)
			throw new ApplicationException("Failed to send email: " + await resp.Body.ReadAsStringAsync(cancellationToken));
		
		string GetMessageBody() => $@"A new update was detected for Lacerte.

<dl>
	<dt>New Version Released At</dt>
	<dd>{ToLocalTime(updatedAt):MMM dd yyyy h:mm:ss tt zzz}</dd>

	<dt>Detected At</dt>
	<dd>{ToLocalTime(detectedAt):MMM dd yyyy h:mm:ss tt zzz}</dd>

	<dt>Next Check At</dt>
	<dd>{ToLocalTime(nextCheckAt):MMM dd yyyy h:mm:ss tt zzz}</dd>
</dl>";
	}
	
	private static DateTime ToLocalTime(DateTime input) => input.ToLocalTime();
}