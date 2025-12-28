using System;
using Hydrogen.Mapping;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Encapsulates how change tracking is performed on objects, optionally mapping to a boolean property for automatic dirty flagging.
/// </summary>
internal class ObjectChangeTracker {

	public ObjectChangeTracker()  {
		HasChanged = _ => false;;
		SetChanged = (_, _) => { };
		SupportsChangeTracking = false;
	}

	public ObjectChangeTracker(Member member) 
		: this(Guard.EnsureCast<PropertyMember>(member, $"Must be a {nameof(PropertyMember)}")) {
	}

	public ObjectChangeTracker(PropertyMember member) {
		Guard.ArgumentNotNull(member, nameof(member));
		Guard.Ensure(member.CanRead, "Member must have public getter");
		Guard.Ensure(member.CanWrite, "Member must have public setter");
		Guard.Ensure(member.PropertyType == typeof(bool), "Member must be of type bool");
		HasChanged = obj => (bool)member.GetValue(obj);
		SetChanged = (obj, val) => member.SetValue(obj, val);
		SupportsChangeTracking = true;
	}

	public ObjectChangeTracker(Func<object, bool> hasChanged, Action<object, bool> setChanged) {
		Guard.ArgumentNotNull(hasChanged, nameof(hasChanged));
		Guard.ArgumentNotNull(setChanged, nameof(setChanged));
		HasChanged = hasChanged;
		SetChanged = setChanged;
		SupportsChangeTracking = true;
	}

	public bool SupportsChangeTracking { get; }

	public Func<object, bool> HasChanged { get; }

	public Action<object, bool> SetChanged { get; }

	public static ObjectChangeTracker Default { get; } = new ();

}
