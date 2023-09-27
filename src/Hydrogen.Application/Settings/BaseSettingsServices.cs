// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Application;

public abstract class BaseSettingsServices : ISettingsServices {
	public event EventHandler ConfigurationChanged {
		add => throw new NotImplementedException();
		remove => throw new NotImplementedException();
	}

	protected virtual void OnConfigurationChanged() => throw new NotImplementedException();

	public void NotifyConfigurationChangedEvent() {
		throw new NotImplementedException();
		//OnConfigurationChanged();
		//ConfigurationChanged(this, EventArgs.Empty);
	}

	public abstract ISettingsProvider UserSettings { get; }

	public abstract ISettingsProvider SystemSettings { get; }
}
