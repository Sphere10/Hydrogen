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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sphere10.Framework.Application {
	
	[XmlRoot]
	[Serializable]
	public abstract class SettingsObject : ObjectWithDefaultValues {

		[DefaultValue(null)]
		public object ID { get; set; }

		[XmlIgnore]
		public ISettingsProvider Provider { get; set; }


		public virtual void Load() {
			ValidateProvider();
			Provider.ReloadSetting(this);
		}


		public virtual void Save() {
			ValidateProvider();
			Provider.SaveSetting(this);
		}

	
		public virtual void Delete() {
			ValidateProvider();
			Provider.DeleteSetting(this);
		}


		private void ValidateProvider() {
			if (Provider == null)
				throw new SoftwareException("Setting cannot be saved as it is not registered with a settings provider.");

		}



	}
}
