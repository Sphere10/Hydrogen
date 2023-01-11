using System.Xml.Serialization;

namespace Hydrogen.Web.AspNetCore;

public enum SitemapFrequency {

	[XmlEnum("never")]
	Never,

	[XmlEnum("yearly")]
	Yearly,

	[XmlEnum("monthly")]
	Monthly,

	[XmlEnum("weekly")]
	Weekly,

	[XmlEnum("daily")]
	Daily,

	[XmlEnum("hourly")]
	Hourly,

	[XmlEnum("always")]
	Always
}
