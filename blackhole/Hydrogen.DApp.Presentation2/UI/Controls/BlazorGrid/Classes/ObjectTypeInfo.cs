// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: David Price
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes {
	public class ObjectTypeInfo {
		public PropertyInfo PropertyInfo { get; set; }
		public Type Type { get; set; }
		public string TypeName { get; set; }
		public string TypeFullName { get; set; }
		public bool IsEnum { get; set; }
		public List<string> EnumValues { get; set; } = new List<string>();

		public ObjectTypeInfo(PropertyInfo propertyInfo) {
			try {
				PropertyInfo = propertyInfo;
				TypeName = PropertyInfo.PropertyType.Name.ToString();
				TypeFullName = PropertyInfo.PropertyType.FullName;
				Type = Type.GetType(TypeFullName);
				IsEnum = Type.IsEnum;
				if (IsEnum) {
					//		var name = Type.Name;
					//		FieldInfo field = Type.GetField(name);
					//		Attribute.GetCustomAttribute(field, Type);

					var enumValues = Enum.GetValues(Type);
					foreach (var enumValue in enumValues) {
						EnumValues.Add(enumValue.ToString());
					}

					//		EnumValues = new List<Type>(Enum.GetValues(Type))).Select(x => x.ToString()).ToList();
				}
			} catch (Exception ex) {

			}
		}

		public ObjectTypeInfo(string name) {
			PropertyInfo = name.GetType().GetProperty("this");
			TypeName = name;
			TypeFullName = name;
			Type = TypeFullName.GetType();
			IsEnum = false;
		}
	}
}
