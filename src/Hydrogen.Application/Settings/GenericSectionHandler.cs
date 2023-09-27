// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace Hydrogen.Application;

// based off https://sites.google.com/site/craigandera/craigs-stuff/clr-workings/the-last-configuration-section-handler-i-ll-ever-need
public class GenericSectionHandler : IConfigurationSectionHandler {

	public static IDictionary<string, string> KnownConfigurationObjectsByLowerCaseSectionName = new Dictionary<string, string>() {
		{ "componentregistry", "Hydrogen.Application.ComponentRegistryDefinition, Hydrogen.Application" }
	};

	public object Create(object parent, object configContext, System.Xml.XmlNode section) {
		var nav = section.CreateNavigator();

		// load configuration type explicitly
		var typeName = nav.Evaluate("string(@type)") as string;
		if (string.IsNullOrWhiteSpace(typeName?.Trim())) {
			// Not specified, use element name to lookup known configuration objects
			var elementName = nav.Name;

			if (KnownConfigurationObjectsByLowerCaseSectionName.ContainsKey(elementName.ToLowerInvariant())) {
				typeName = KnownConfigurationObjectsByLowerCaseSectionName[elementName.ToLowerInvariant()];
			} else {
				typeName = TypeResolver.Resolve(elementName).AssemblyQualifiedName;
				//throw new SoftwareException("No known configuration object for '{0}'", elementName);
			}
		}
		var configurationObjectType = TypeResolver.Resolve(typeName);
		var ser = new XmlSerializer(configurationObjectType);
		return ser.Deserialize(new XmlNodeReader(section));
	}

	public static T GetConfig<T>(string sectionName = null) where T : class {
		if (sectionName == null)
			sectionName = typeof(T).Name;
		var configurationSection = ConfigurationManager.GetSection(sectionName) as T;
		if (configurationSection == null)
			throw new SoftwareException($"Unable to load {typeof(T).Name}, make sure web.config (or app.config) contains this section");
		return configurationSection;
	}

}
