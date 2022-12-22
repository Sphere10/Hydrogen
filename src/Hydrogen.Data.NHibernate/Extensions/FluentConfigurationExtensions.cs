using FluentNHibernate.Cfg;
using NHibernate.Event;

namespace Hydrogen.Data.NHibernate;
public static class FluentConfigurationExtensions {
	public static FluentConfiguration AddBusinessObjectAuditor(this FluentConfiguration fluentConfiguration) {
		var auditor = new BusinessObjectAuditor();
		return fluentConfiguration.ExposeConfiguration(c => c.AppendListeners(ListenerType.PreInsert, new IPreInsertEventListener [] { auditor }))
						  .ExposeConfiguration(c => c.AppendListeners(ListenerType.PreUpdate, new IPreUpdateEventListener [] { auditor }));
						  //.ExposeConfiguration(c => c.AppendListeners(ListenerType.PreLoad, new IPreLoadEventListener [] { sealer }));
	}
}