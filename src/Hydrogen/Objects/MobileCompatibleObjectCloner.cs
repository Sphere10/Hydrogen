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

using System.Linq;
using System.Reflection;

#if USE_FAST_REFLECTION
using Hydrogen.FastReflection;
#endif

namespace Hydrogen {

	public class MobileCompatibleObjectCloner : IObjectCloner {

		public void Copy(object source, object dest) {
			var bindingFlags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.SetField;
			// HS: Removed 2019-02-19, .NET Standard 2.0 assume has unrestricted
			//if (Tools.CodeAccessSecurity.HasUnrestrictedFeatureSet)
			//    bindingFlags |= BindingFlags.NonPublic;
			bindingFlags |= BindingFlags.NonPublic;

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
				fieldBinding.DestField.SetValue(dest, fieldBinding.SourceField.FastGetValue(source)); // using FastReflection lib		
#else
                fieldBinding.DestField.SetValue(dest, fieldBinding.SourceField.GetValue(source)); // using standrad Reflection
#endif
			}

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
				propertyBinding.DestProperty.FastSetValue(dest, propertyBinding.SourceProperty.FastGetValue(source)); // using FastReflection lib			
#else
                propertyBinding.DestProperty.SetValue(dest, propertyBinding.SourceProperty.GetValue(source, null), null);  // using standard Reflection			
#endif
			}

		}

		public object Clone(object obj) {
			var clone = Tools.Object.Create(obj.GetType());
			Copy(obj, clone);
			return clone;
		}

		public T Clone<T>(T obj) {
			return (T)Clone((object)obj);
		}
	}
}
