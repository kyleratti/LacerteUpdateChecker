using Newtonsoft.Json;

namespace ApiPoller.Models;

public record ApplicationState(
	[JsonProperty("lastUpdateCheck")] DateTime? LastCheckAt,
	[JsonProperty("lastKnownVersionTimestamp")] DateTime? LastKnownVersionTimestamp
);