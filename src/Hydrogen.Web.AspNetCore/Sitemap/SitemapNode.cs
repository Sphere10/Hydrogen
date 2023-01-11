using System;
using System.Xml.Serialization;

namespace Hydrogen.Web.AspNetCore;

public class SitemapNode {

	[XmlElement("loc")]
	public string Url { get; set; }

	[XmlElement("lastmod", DataType = "date")]
	public DateTime? LastModified { get; set; }

	[XmlElement("changefreq")]
	public SitemapFrequency? Frequency { get; set; }
	
	[XmlElement("priority")]
	public double? Priority { get; set; }

	public bool LastModifiedSpecified => LastModified != null;

	public bool FrequencySpecified => Frequency != null;

	public bool PrioritySpecified => Priority != null;
	
}
