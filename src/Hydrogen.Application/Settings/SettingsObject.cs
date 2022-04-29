//-----------------------------------------------------------------------
// <copyright file="SettingsObject.cs" company="Sphere 10 Software">
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

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Hydrogen.Application {

	[XmlRoot]
	[Serializable]
	public abstract class SettingsObject : ObjectWithDefaultValues  {

		public object ID { get; set; } = null;

		[XmlIgnore]
		public ISettingsProvider Provider { get; set; }

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

		public virtual Result Validate() => Result.Valid;

		private void CheckProvider() {
			if (Provider == null)
				throw new SoftwareException("Setting cannot be saved as it is not registered with a settings provider.");
		}

	}
}
