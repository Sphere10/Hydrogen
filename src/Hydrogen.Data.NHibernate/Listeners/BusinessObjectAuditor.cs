// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;
using System.Threading.Tasks;
using Hydrogen.FastReflection;
using NHibernate.Event;

namespace Hydrogen.Data.NHibernate;

// TODO: Add CreatedBy and LastUpdatedBy similar to https://ayende.com/blog/3987/nhibernate-ipreupdateeventlistener-ipreinserteventlistener
// Get a IAuditableIdentityProvider from ComponentRegistry to properly support ASP.NET
internal class BusinessObjectAuditor : IPreInsertEventListener, IPreUpdateEventListener {

	public Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
		=> Task.FromResult(OnPreInsert(@event));


	public bool OnPreInsert(PreInsertEvent @event) {
		var businessObject = @event.Entity;
		if (businessObject != null) {
			var now = DateTime.UtcNow;
			//businessObject.CreatedOnUtc = DateTime.UtcNow;
			//businessObject.LastUpdatedOnUtc = DateTime.UtcNow;
			//businessObject.Active = true;
			//ListenerHelper.SetProperty(@event.Persister, @event.State, "CreatedOnUtc", businessObject.CreatedOnUtc);
			//ListenerHelper.SetProperty(@event.Persister, @event.State, "LastUpdatedOnUtc", businessObject.LastUpdatedOnUtc);
			//ListenerHelper.SetProperty(@event.Persister, @event.State, "Active", businessObject.Active);                
			var createdOnUtcProp = businessObject.GetType().GetProperty("CreatedOnUtc");
			if (createdOnUtcProp != null) {
				createdOnUtcProp.FastSetValue(businessObject, now);
				ListenerHelper.SetProperty(@event.Persister, @event.State, "CreatedOnUtc", now);
			}

			var lastUpdatedOnUtcProp = businessObject.GetType().GetProperty("LastUpdatedOnUtc");
			if (lastUpdatedOnUtcProp != null) {
				lastUpdatedOnUtcProp.FastSetValue(businessObject, now);
				ListenerHelper.SetProperty(@event.Persister, @event.State, "LastUpdatedOnUtc", now);
			}

			var activeProp = businessObject.GetType().GetProperty("Active");
			if (activeProp != null) {
				activeProp.FastSetValue(businessObject, true);
				ListenerHelper.SetProperty(@event.Persister, @event.State, "Active", true);
			}

		}
		return false;
	}

	public Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken cancellationToken)
		=> Task.FromResult(OnPreUpdate(@event));

	public bool OnPreUpdate(PreUpdateEvent @event) {
		//var businessObject = @event.Entity as BusinessObject;
		//if (businessObject != null) {
		//    businessObject.LastUpdatedOnUtc = DateTime.UtcNow;
		//    ListenerHelper.SetProperty(@event.Persister, @event.State, "LastUpdatedOnUtc", businessObject.LastUpdatedOnUtc);
		//}
		var businessObject = @event.Entity;
		if (businessObject != null) {
			var now = DateTime.UtcNow;

			var lastUpdatedOnUtcProp = businessObject.GetType().GetProperty("LastUpdatedOnUtc");
			if (lastUpdatedOnUtcProp != null) {
				lastUpdatedOnUtcProp.FastSetValue(businessObject, now);
				ListenerHelper.SetProperty(@event.Persister, @event.State, "LastUpdatedOnUtc", now);
			}

		}

		return false;
	}

}
