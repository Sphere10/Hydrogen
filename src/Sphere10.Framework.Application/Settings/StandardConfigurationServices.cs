//-----------------------------------------------------------------------
// <copyright file="StandardConfigurationServices.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Application {


	public class StandardConfigurationServices : BaseConfigurationServices  {

		public StandardConfigurationServices() {
			ResolveSettingsImplementations();
		}

		public ISettingsProvider UserSettingsImpl { get; private set; }
		public ISettingsProvider SystemSettingsImpl { get; private set; }


		protected virtual void ResolveSettingsImplementations() {
			UserSettingsImpl = ComponentRegistry.Instance.Resolve<ISettingsProvider>("UserSettings");
			SystemSettingsImpl = ComponentRegistry.Instance.Resolve<ISettingsProvider>("SystemSettings");
		}

		public override ISettingsProvider UserSettings => UserSettingsImpl;


		public override ISettingsProvider SystemSettings => SystemSettingsImpl;
	}
}
