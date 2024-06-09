// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

#define USE_FAST_REFLECTION

using System.ComponentModel;
using System.Reflection;
using Hydrogen.FastReflection;


namespace Hydrogen;

public abstract class ObjectWithDefaultValues : object {
	protected ObjectWithDefaultValues()
		: this(true) {
	}

	protected ObjectWithDefaultValues(bool setDefaultValues) {
		if (setDefaultValues)
			RestoreDefaultValues();
	}

	public virtual void RestoreDefaultValues() {
		// Default implementation is to use 
		Tools.Object.SetDefaultValues(this);
	}

	public static void SetDefaults(object obj) {
		var bindingFlags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.SetField;
		// HS: Removed 2019-02-19, .NET Standard 2.0 assume has unrestricted
		//if (Tools.CodeAccessSecurity.HasUnrestrictedFeatureSet)
		//    bindingFlags |= BindingFlags.NonPublic;
		bindingFlags |= BindingFlags.NonPublic;

		foreach (var f in obj.GetType().GetFields(bindingFlags)) {
			foreach (var attr in f.GetCustomAttributesOfType<DefaultValueAttribute>()) {
				var dv = (DefaultValueAttribute)attr;
				f.SetValue(obj, Tools.Object.ChangeType(dv.Value, f.FieldType));
			}
		}

		bindingFlags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.SetProperty;
		// HS: Removed 2019-02-19, .NET Standard 2.0 assume has unrestricted
		//if (Tools.CodeAccessSecurity.HasUnrestrictedFeatureSet)
		//    bindingFlags |= BindingFlags.NonPublic;
		bindingFlags |= BindingFlags.NonPublic;

		foreach (var p in obj.GetType().GetProperties(bindingFlags)) {
			if (p.GetIndexParameters().Length != 0)
				continue;

			if (!p.CanWrite)
				continue;

			foreach (var attr in p.GetCustomAttributesOfType<DefaultValueAttribute>()) {
#if USE_FAST_REFLECTION
				p.FastSetValue(obj, Tools.Object.ChangeType(attr.Value, p.PropertyType)); // using FastReflection lib
#else
                    p.SetValue(obj, TypeChanger.ChangeType(attr.Value, p.PropertyType), null);		// using normal reflection
#endif
			}
		}
	}
}
