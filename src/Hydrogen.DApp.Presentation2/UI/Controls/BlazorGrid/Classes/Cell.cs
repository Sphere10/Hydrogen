using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes {
	public class Cell 
	{
		public HeaderData Header { get; private set; }
		public RowData Row { get; private set; }
		public ObjectTypeInfo ObjectTypeInfo { get; private set; }
		public string HeaderText { get; private set; }
		public object Tag { get; private set; } // this is the full row of data
		public object CellData { get; private set; }
		public int DataIndex { get; private set; }
		public PropertyInfo PropertyInfo { get; private set; }
		public bool IsHeader { get; private set;}
		public string Text { get; private set; }

		public string Name { get { return Header.Name; } }
		public int Width { get { return Header.Width; } }
		public int Height { get { return Row.Height; } }
		public bool IsEnum { get; private set; }
		public bool IsCollection { get; private set; }
		public List<string> ICollectionValues { get; private set; }
		public string CollectionId { get; set; }

		public Cell(HeaderData header, RowData row, ObjectTypeInfo typeInfo, string headerText, object underlyingData, int dataIndex, PropertyInfo propertyInfo, bool isHeader) 
		{ 
			Header = header;
			Row = row;
			ObjectTypeInfo = typeInfo;
			HeaderText = headerText;
			Tag = underlyingData;
			DataIndex = dataIndex;
			PropertyInfo = propertyInfo;
			IsHeader = isHeader;
			IsEnum = ObjectTypeInfo.IsEnum;

			if (!IsHeader) {
				var value = PropertyInfo.GetValue(Tag);
				var namespaces = PropertyInfo.PropertyType.GetInterfaces().Select(x => x.Namespace).ToList();
				IsCollection = namespaces.Any(x => x == "System.Collections" || x == "System.Collections.Generic") && value is not string;
				if (IsCollection) {
					ICollectionValues = ((IEnumerable<object>)value).Select(x => x.ToString()).ToList();
					CollectionId = Guid.NewGuid().ToString();
				}
			}

			Text = GetCellText();
		}

		public string GetInputType() 
		{
			if (IsCollection) {

			}

			switch (ObjectTypeInfo.Type.Name.ToLower()) 
			{
				case "datetime":	return "date";
				case "decimal":
				case "double":
				case "float":
				case "long":
				case "int32":
				case "int64":		return "number";
				case "bool":		return "checkbox";
				default:			return "text";
			}
		}

		public string GetListName() 
		{
			if (ObjectTypeInfo.IsEnum) 
			{
				return Name;
			}

			if (IsCollection) {

			}

			return string.Empty;
		}

		public string GetCellText() {
			if (IsCollection) {
				return String.Empty;
			}

			if (IsHeader) {
				return HeaderText;
			}

			var value = PropertyInfo.GetValue(Tag);
			var typeName = PropertyInfo.PropertyType.Name.ToString();
			switch (typeName) {
				case "DateTime": return ((DateTime)value).ToString("yyyy-MM-dd");
				default: return value.ToString();
			}
		}

		public void UpdateData(string newValue) 
		{
			var objectValue = Tools.Parser.Parse(ObjectTypeInfo.Type, newValue);
			ObjectTypeInfo.PropertyInfo.SetValue(Tag, objectValue);
/*
			object objectValue = null;

			if (IsEnum) 
			{
				objectValue = Enum.Parse(TypeInfo.Type, newValue);
			} 
			else 
			{
				switch (TypeInfo.TypeName.ToLower()) 
				{
					case "string": objectValue = newValue; break;
					case "datetime": objectValue = DateTime.Parse(newValue); break;
					case "decimal": objectValue = decimal.Parse(newValue); break;
					case "double": objectValue = double.Parse(newValue); break;
					case "float":
					case "single:": objectValue = float.Parse(newValue); break;
					case "long":
					case "int64": objectValue = long.Parse(newValue); break;
					case "int":
					case "int32": objectValue = int.Parse(newValue); break;
					case "uint": objectValue = uint.Parse(newValue); break;
					case "short":
					case "int16": objectValue = short.Parse(newValue); break;
					case "ushort": objectValue = ushort.Parse(newValue); break;
					case "bool": objectValue = bool.Parse(newValue); break;
					case "char": objectValue = char.Parse(newValue); break;
					case "byte": objectValue = byte.Parse(newValue); break;
					case "sbyte": objectValue = sbyte.Parse(newValue); break;
					default: throw new Exception("Cell.UpdateData Unknown Data Type; ");
				}
			}

			// handle arrays, IEnumerable, classes, trees of classes
			TypeInfo.PropertyInfo.SetValue(Tag, objectValue);
*/
		}
	}
}