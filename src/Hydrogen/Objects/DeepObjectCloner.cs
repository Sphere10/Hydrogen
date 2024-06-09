// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

#define USE_FAST_REFLECTION
#if __IOS__
#undef USE_FAST_REFLECTION
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if USE_FAST_REFLECTION
using Hydrogen.FastReflection;
#endif

namespace Hydrogen {

	public class DeepObjectCloner : IObjectCloner {

		protected readonly HashSet<Type> DontCloneTypes;

		public DeepObjectCloner() : this(Enumerable.Empty<Type>()) {
		}

		public DeepObjectCloner(params Type[] dontClone) : this(dontClone.ToList()) {
		}

		public DeepObjectCloner(IEnumerable<Type> dontClone) {
			DontCloneTypes = new HashSet<Type> { typeof(string) };
			if (dontClone != null)
				DontCloneTypes.AddRange(dontClone);
		}

		public virtual object Clone(object source) {
			return DeepClone(source, new Dictionary<Reference<object>, object>());
		}

		public virtual void Copy(object source, object dest) {
			DeepCopyMembers(source, dest, new Dictionary<Reference<object>, object>());
		}

		protected virtual object ActivateObject(Type type, object sourceObjectHint) {
			return Tools.Object.Create(type); // create object    
		}

		protected virtual object DeepClone(object source, IDictionary<Reference<object>, object> clones) {
			// This method will clone the object if not already cloned
			var key = Reference.For(source);
			if (!clones.ContainsKey(key)) {
				var dest = ActivateObject(source.GetType(), source);
				clones.Add(key, dest); // register empty object as clone (handles cyclic dependencies)
				DeepCopyMembers(source, dest, clones); // copy the members
			}
			return clones[key];
		}

		protected virtual void DeepCopyMembers(object source, object dest, IDictionary<Reference<object>, object> clones) {
			if (!source.GetType().IsInstanceOfType(dest))
				throw new ArgumentException("Source and dest types do not match", "dest");

			// Avoid cyclic-dependencies
			//if (clones.ContainsKey( new ReferencedObject<object>(dest)))
			//	throw new Exception("Circular cloning detected, aborting");

			// Copy everythig (including private members if possible)
			var bindingFlags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.SetField;
			// HS: Removed 2019-02-19, .NET Standard 2.0 assume has unrestricted
			//if (Tools.CodeAccessSecurity.HasUnrestrictedFeatureSet)
			//    bindingFlags |= BindingFlags.NonPublic;
			bindingFlags |= BindingFlags.NonPublic;

			#region Copy Fields

			var sourceFields = source.GetType().GetFields(bindingFlags);
			var destFields = dest.GetType().GetFields(bindingFlags);

			var fieldBindings =
				from sourceField in sourceFields
				join destField in destFields on sourceField.Name equals destField.Name
				where !sourceField.FieldType.IsNested && !destField.FieldType.IsNested && destField.FieldType.IsAssignableFrom(sourceField.FieldType)
				select new {
					SourceField = sourceField,
					DestField = destField
				};

			foreach (var fieldBinding in fieldBindings) {
#if USE_FAST_REFLECTION
				var sourceValue = fieldBinding.SourceField.FastGetValue(source);
#else
			    var sourceValue = fieldBinding.SourceField.GetValue(source);
#endif
				if (fieldBinding.DestField.FieldType.IsValueType || DontCloneTypes.Contains(fieldBinding.DestField.FieldType) || sourceValue == null) {
#if USE_FAST_REFLECTION
					fieldBinding.DestField.SetValue(dest, sourceValue); // using FastReflection lib
#else
					fieldBinding.DestField.SetValue(dest, sourceValue); // using standrad Reflection
#endif
				} else {
					sourceValue = DeepClone(sourceValue, clones);

#if USE_FAST_REFLECTION
					fieldBinding.DestField.SetValue(dest, sourceValue);
#else
					fieldBinding.DestField.SetValue(dest, sourceValue); // using standrad Reflection
#endif
				}
			}

			#endregion

			#region Copy Properties

			bindingFlags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.SetProperty;
			// HS: Removed 2019-02-19, .NET Standard 2.0 assume has unrestricted
			//if (Tools.CodeAccessSecurity.HasUnrestrictedFeatureSet)
			//    bindingFlags |= BindingFlags.NonPublic;
			bindingFlags |= BindingFlags.NonPublic;

			var sourceProperties = source.GetType().GetProperties(bindingFlags);
			var destProperties = dest.GetType().GetProperties(bindingFlags);

			var propertyBindings =
				from sourceProperty in sourceProperties
				join destProperty in destProperties on sourceProperty.Name equals destProperty.Name
				where
					sourceProperty.GetIndexParameters().Length == 0 && sourceProperty.CanRead &&
					destProperty.GetIndexParameters().Length == 0 && destProperty.CanWrite &&
					destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType)
				select new {
					SourceProperty = sourceProperty,
					DestProperty = destProperty
				};

			foreach (var propertyBinding in propertyBindings) {
#if USE_FAST_REFLECTION
				var sourceValue = propertyBinding.SourceProperty.FastGetValue(source);
#else
			    var sourceValue = propertyBinding.SourceProperty.GetValue(source);
#endif
				if (propertyBinding.DestProperty.PropertyType.IsValueType || DontCloneTypes.Contains(propertyBinding.DestProperty.PropertyType) || sourceValue == null) {
#if USE_FAST_REFLECTION
					propertyBinding.DestProperty.FastSetValue(dest, sourceValue); // using FastReflection lib			
#else
					propertyBinding.DestProperty.SetValue(dest, sourceValue, null);  // using standard Reflection
#endif
				} else {
					sourceValue = DeepClone(sourceValue, clones);
#if USE_FAST_REFLECTION
					propertyBinding.DestProperty.FastSetValue(dest, sourceValue);
#else
					propertyBinding.DestProperty.SetValue(dest, sourceValue, null);  // using standard Reflection
#endif
				}
			}

			#endregion

		}


		protected virtual bool ShouldDeepCopyType(Type type) {
			return !DontCloneTypes.Contains(type);
		}
	}
}
