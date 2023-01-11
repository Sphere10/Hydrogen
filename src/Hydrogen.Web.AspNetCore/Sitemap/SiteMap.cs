using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Hydrogen.Web.AspNetCore;

[XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
public class SiteMap {
	private Dictionary<string, SitemapNode> _nodes = new();

	public bool HasNode(string url) => _nodes.ContainsKey(url);
	public void Add(string url, DateTime? lastModified = null, SitemapFrequency? frequency = null, double? priority = null)
		=> _nodes.Add(
			url, 
			new SitemapNode { 
				Url = url,
				LastModified = lastModified,
				Frequency = frequency,
				Priority = priority
			}
		);

	[XmlElement("url", typeof(SitemapNode))]
	public SitemapNode[] Nodes { 
		get => _nodes.Values.ToArray();
		set => _nodes = value != null ? value.ToDictionary(x => x.Url) : new();
	} 
}