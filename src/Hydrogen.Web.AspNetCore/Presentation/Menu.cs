// <copyright file="Menu.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System.Xml.Serialization;

namespace Hydrogen.Web.AspNetCore {

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

		[XmlAttribute("text")]
		public string Text { get; set; }

		[XmlAttribute("url")]
		public string Url { get; set; }

		[XmlAttribute("icon")]
		public string Icon { get; set; }

		[XmlAttribute("glyph")]
		public string Glyph { get; set; }

		[XmlElement("Menu")]
		public Menu[] SubMenus { get; set; }

	}

}
