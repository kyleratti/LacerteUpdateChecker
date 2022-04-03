namespace ApiPoller.Models;

/// <summary>
/// .
/// </summary>
/// <param name="Value">An XML-encoded value representing the actual current version settings</param>
/// <param name="Code"></param>
public class GetUpdateManifestResponse
{
	public string Value { get; set; }
	public int Code { get; set; }
}