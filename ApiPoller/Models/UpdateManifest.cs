using System.Xml.Serialization;

namespace ApiPoller.Models;

[Serializable, XmlRoot("manifest")]
public class UpdateManifest
{
	[XmlElement("product")]
	public Product[] Products { get; set; }
}

public class Component
{
	[XmlAttribute] public string Name { get; set; }
	[XmlAttribute] public int Version { get; set; }
	[XmlAttribute("FileSize")] public int FileSizeBytes { get; set; }
	[XmlAttribute] public string FileHash { get; set; }
	[XmlAttribute("FileTime")] public DateTime LastUpdatedAt { get; set; }
}

public class Release
{
	[XmlAttribute] public int MinorRelease { get; set; }
	[XmlAttribute] public int IncrementalRelease { get; set; }
	[XmlAttribute] public int Type { get; set; }
	[XmlAttribute] public string Location { get; set; }
	/// <summary>
	/// This appears to be a generic build time and not when this was actually last updated.
	/// </summary>
	[XmlAttribute] public DateTime BuildTime { get; set; }
	[XmlElement("component")] public Component[] Components { get; set; }
}

public class Product
{
	[XmlAttribute] public string Name { get; set; }
	[XmlAttribute] public int Version { get; set; }
	/// <summary>
	/// This seems to be the time the manifest file was originally created, not when the current version was created
	/// </summary>
	[XmlAttribute] public DateTime CreateTime { get; set; }
	[XmlElement("release")] public Release[] Releases { get; set; }
}