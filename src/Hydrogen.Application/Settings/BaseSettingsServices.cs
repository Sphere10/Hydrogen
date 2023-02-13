//-----------------------------------------------------------------------
// <copyright file="BaseSettingsServices.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Application;

public abstract class BaseSettingsServices : ISettingsServices {
	public event EventHandler ConfigurationChanged { add => throw new NotImplementedException(); remove => throw new NotImplementedException(); }

	protected virtual void OnConfigurationChanged() => throw new NotImplementedException();

	public void NotifyConfigurationChangedEvent() {
		throw new NotImplementedException();
		//OnConfigurationChanged();
		//ConfigurationChanged(this, EventArgs.Empty);
	}

	public abstract ISettingsProvider UserSettings { get; }

	public abstract ISettingsProvider SystemSettings { get; }
}