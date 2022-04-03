using System.Net;
using System.Xml.Serialization;
using ApiPoller.Config;
using ApiPoller.Models;
using ApiPoller.Services;
using DesteSoft.SoapClient;
using Newtonsoft.Json;

namespace ApiPoller;

public class PollWorker : BackgroundService
{
	private readonly ILogger<PollWorker> _logger;
	private readonly LacerteApiSettings _appSettings;
	private readonly GetUpdateManifestActionSettings _getUpdateManifestAction;
	private readonly Emailer _emailer;

	private readonly TimeSpan _pollingInterval = TimeSpan.FromMinutes(5);

	public PollWorker(ILogger<PollWorker> logger, LacerteApiSettings appSettings, GetUpdateManifestActionSettings getUpdateManifestAction, Emailer emailer)
	{
		_logger = logger;
		_appSettings = appSettings;
		_getUpdateManifestAction = getUpdateManifestAction;
		_emailer = emailer;
	}

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		using var client = new SoapClient(_appSettings.ApiUrl, _appSettings.ApiNamespace);

		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				// Yes this is poorly structured. No I don't care.
				await DoRun(cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error completing run");
			}

			await WaitForNextRun(cancellationToken);
		}
	}

	private async Task DoRun(CancellationToken cancellationToken)
	{
		using var client = new SoapClient(_appSettings.ApiUrl, _appSettings.ApiNamespace);

		var requestParams = new GetUpdateManifestRequest(new[]
			{
				new ProductRequest(
					_getUpdateManifestAction.ProductName,
					_getUpdateManifestAction.ProductVersion,
					_getUpdateManifestAction.MinorVersion,
					_getUpdateManifestAction.IncrementalVersion,
					_getUpdateManifestAction.ExcludeLocations
				)
			},
			new Source(_getUpdateManifestAction.SourceName)
		);

		var checkStartedAt = DateTime.UtcNow;

		// Don't know why, but the payload needs "mr" wrapped around it
		var req = await client.PostAsync<GetUpdateManifestResponse>(_getUpdateManifestAction.ActionName, new {
			mr = requestParams
		});

		var xml = WebUtility.HtmlDecode(req.Value);
		var components = ParseComponentsFromUpdateManifest(xml);

		if (components is null)
			return;

		var newestComponent = components.MaxBy(x => x.LastUpdatedAt)!;
		_logger.LogInformation("Found newest component: {ReleaseDate}", newestComponent.LastUpdatedAt);

		var lastKnownState = await GetApplicationState();
		_logger.LogDebug("Last known component age: {LastAge}", lastKnownState?.LastCheckAt);

		if (lastKnownState?.LastKnownVersionTimestamp is not null &&
		    lastKnownState.LastKnownVersionTimestamp.Value <= newestComponent.LastUpdatedAt)
		{
			_logger.LogDebug("Newest component is <= last known component age; skipping");
			return;
		}
		
		_logger.LogInformation("Found new component version");
		await _emailer.SendNewVersionNotification(
			detectedAt: checkStartedAt,
			newestComponent.LastUpdatedAt,
			DateTime.UtcNow + _pollingInterval,
			cancellationToken);
		_logger.LogInformation("Sent e-mail notification");
		await SetApplicationState(new ApplicationState(DateTime.UtcNow, newestComponent.LastUpdatedAt));
	}
	
	private async Task WaitForNextRun(CancellationToken cancellationToken) => await Task.Delay(_pollingInterval, cancellationToken);

	private async Task SetApplicationState(ApplicationState newState) =>
		await File.WriteAllTextAsync(_appSettings.ApplicationStatePath, JsonConvert.SerializeObject(newState));

	private async Task<ApplicationState?> GetApplicationState()
	{
		if (!File.Exists(_appSettings.ApplicationStatePath))
		{
			return null;
		}

		var contents = await File.ReadAllTextAsync(_appSettings.ApplicationStatePath);

		if (string.IsNullOrEmpty(contents))
		{
			return null;
		}

		return JsonConvert.DeserializeObject<ApplicationState>(contents);
	}

	private IReadOnlyCollection<Component>? ParseComponentsFromUpdateManifest(string xml)
	{
		using var reader = new StringReader(xml);
		var serializer = new XmlSerializer(typeof(UpdateManifest));
		var result = (UpdateManifest?)serializer.Deserialize(reader);
		
		if (result is null)
		{
			_logger.LogError("Unable to deserialize object: {Xml}", xml);
			return null;
		}

		if (result.Products.Length != 1)
		{
			_logger.LogError("Expected 1 product, found {ProductCount}", result.Products.Length);
			return null;
		}

		var product = result.Products.First();

		if (product.Releases.Length != 1)
		{
			_logger.LogError("Expected 1 release, found {ReleaseCount}", product.Releases.Length);
			return null;
		}

		var release = product.Releases.First();

		if (release.Components.Length <= 0)
		{
			_logger.LogError("Expected 1+ components, found {ComponentCount}", release.Components.Length);
			return null;
		}

		return release.Components;
	}
}