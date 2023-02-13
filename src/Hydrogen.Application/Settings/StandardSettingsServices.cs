//-----------------------------------------------------------------------
// <copyright file="StandardSettingsServices.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Application;


public class StandardSettingsServices : BaseSettingsServices {

	public StandardSettingsServices(Global<ISettingsProvider> globalSettings, Local<ISettingsProvider> userSettings) {
		SystemSettings = globalSettings.Item;
		UserSettings = userSettings.Item;
	}

	public override ISettingsProvider SystemSettings { get; }

	public override ISettingsProvider UserSettings { get; }

}
