namespace ApiPoller.Models;

public record GetUpdateManifestRequest(
	ProductRequest[] Requests,
	Source Source
);

public record ProductRequest(
	string ProductName,
	int ProductVersion,
	int MinorVersion,
	int IncrementalVersion,
	bool ExcludeLocations
);

public record Source(string SourceName);