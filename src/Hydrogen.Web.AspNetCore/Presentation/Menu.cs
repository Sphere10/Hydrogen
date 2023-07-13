// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Xml.Serialization;

namespace Hydrogen.Web.AspNetCore;

[XmlRoot("Menu")]
public class Menu {

	public Menu() {
		SubMenus = new Menu[0];
		Url = "#";
	}

	public Menu(string text, string url, string glyph = null) : this() {
		Text = text;
		Url = url;
		Glyph = glyph;
	}

	[XmlAttribute("text")] public string Text { get; set; }

	[XmlAttribute("url")] public string Url { get; set; }

	[XmlAttribute("icon")] public string Icon { get; set; }

	[XmlAttribute("glyph")] public string Glyph { get; set; }

	[XmlElement("Menu")] public Menu[] SubMenus { get; set; }

}
