// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: David Price
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Reflection;

namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes {
	public class Cell {
		public HeaderData Header { get; set; }
		public RowData Row { get; set; }
		public ObjectTypeInfo TypeInfo { get; set; }
		public string Text { get; set; }
		public object Tag { get; private set; } // this is the full row of data
		public int DataIndex { get; set; }

		public string Name {
			get { return Header.Name; }
		}

		public int Width {
			get { return Header.Width; }
		}

		public int Height {
			get { return Row.Height; }
		}

		public bool IsEnum { get; set; }

		public Cell(HeaderData header, RowData row, ObjectTypeInfo typeInfo, string text, object underlyingData, int dataIndex) {
			Header = header;
			Row = row;
			TypeInfo = typeInfo;
			Text = text;
			Tag = underlyingData;
			DataIndex = dataIndex;
			IsEnum = TypeInfo.IsEnum;
		}

/*
		public Cell(HeaderData header, RowData row, ObjectTypeInfo typeInfo, PropertyInfo propertyInfo, int dataIndex) 
			:this(header, row, typeInfo, propertyInfo, header.Name, dataIndex)
		{
		}

		public Cell(HeaderData header, RowData row, ObjectTypeInfo typeInfo, PropertyInfo propertyInfo, object underlyingData, int dataIndex)
		{
			Header = header;
			Row = row;
			TypeInfo = typeInfo;
			PropertyInfo = propertyInfo;
			Tag = underlyingData;
			DataIndex = dataIndex;
			IsEnum = TypeInfo.IsEnum;

			Text = GetText();
		}
*/
		public string GetInputType() {
			switch (TypeInfo.Type.Name.ToLower()) {
				case "datetime": return "date";
				case "decimal":
				case "double":
				case "float":
				case "long":
				case "int32":
				case "int64": return "number";
				case "bool": return "checkbox";
				default: return "text";
			}
		}

		public string GetListName() {
			if (TypeInfo.IsEnum) {
				return Name;
			}

			return string.Empty;
		}

		// improve this, they way it is called, maybe store the data on the cell
		public static string GetCellText(object cellData, PropertyInfo property) {
			var value = property.GetValue(cellData);
			var typeName = property.PropertyType.Name.ToString();
			switch (typeName) {
				case "DateTime": return ((DateTime)value).ToString("yyyy-MM-dd");
				default: return value.ToString();
			}
		}
		/*
				public string GetText() 
				{
					object value;
					string typeName;

					try 
					{
						value = PropertyInfo.GetValue(Tag);
						typeName = PropertyInfo.PropertyType.Name.ToString();
						switch (typeName) 
						{
							case "DateTime": return ((DateTime)value).ToString("yyyy-MM-dd");
							default: return value.ToString();
						}
					}
					catch (Exception ex) 
					{
						return "Error";
					}
				}
		*/
		public void UpdateData(string newValue) {
			var objectValue = Tools.Parser.Parse(TypeInfo.Type, newValue);
			TypeInfo.PropertyInfo.SetValue(Tag, objectValue);
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
