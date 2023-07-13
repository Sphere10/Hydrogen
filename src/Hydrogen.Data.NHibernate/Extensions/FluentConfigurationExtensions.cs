// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using FluentNHibernate.Cfg;
using NHibernate.Event;

namespace Hydrogen.Data.NHibernate;

public static class FluentConfigurationExtensions {
	public static FluentConfiguration AddBusinessObjectAuditor(this FluentConfiguration fluentConfiguration) {
		var auditor = new BusinessObjectAuditor();
		return fluentConfiguration.ExposeConfiguration(c => c.AppendListeners(ListenerType.PreInsert, new IPreInsertEventListener[] { auditor }))
			.ExposeConfiguration(c => c.AppendListeners(ListenerType.PreUpdate, new IPreUpdateEventListener[] { auditor }));
		//.ExposeConfiguration(c => c.AppendListeners(ListenerType.PreLoad, new IPreLoadEventListener [] { sealer }));
	}
}
