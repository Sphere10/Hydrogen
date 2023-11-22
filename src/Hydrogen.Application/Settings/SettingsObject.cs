// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Hydrogen.Application;

[XmlRoot]
[Serializable]
public abstract class SettingsObject : ObjectWithDefaultValues {

	[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
	public object ID { get; set; } = null;

	[XmlIgnore] [JsonIgnore] public ISettingsProvider Provider { get; set; }

	public virtual void Load() {
		CheckProvider();
		Provider.ReloadSetting(this);
	}

	public virtual void Save() {
		CheckProvider();
		Provider.SaveSetting(this);
	}

	public virtual void Delete() {
		CheckProvider();
		Provider.DeleteSetting(this);
	}

	public virtual Result Validate() => Result.Success;

	private void CheckProvider() {
		if (Provider == null)
			throw new SoftwareException("Setting cannot be saved as it is not registered with a settings provider.");
	}

}
