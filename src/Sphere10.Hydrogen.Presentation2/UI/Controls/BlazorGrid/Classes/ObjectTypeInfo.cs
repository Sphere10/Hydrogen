using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Classes {
	public class ObjectTypeInfo 
	{
		public Type Type { get; set; }
		public string TypeName { get; set; }
		public string TypeFullName { get; set; }
		public bool IsEnum { get; set; }
		public List<string> EnumValues { get; set; } = new List<string>();

		public ObjectTypeInfo(PropertyInfo propertyInfo) 
		{
			try 
			{
				TypeName = propertyInfo.PropertyType.Name.ToString();
				TypeFullName = propertyInfo.PropertyType.FullName;
				Type = Type.GetType(TypeFullName);
				IsEnum = Type.IsEnum;
				if (IsEnum) 
				{
			//		var name = Type.Name;
			//		FieldInfo field = Type.GetField(name);
			//		Attribute.GetCustomAttribute(field, Type);

					var enumValues = Enum.GetValues(Type);
					foreach (var enumValue in enumValues)
					{
						EnumValues.Add(enumValue.ToString());
					}

			//		EnumValues = new List<Type>(Enum.GetValues(Type))).Select(x => x.ToString()).ToList();
				}
			}
			catch (Exception ex) 
			{

			}
		}
	}
}
