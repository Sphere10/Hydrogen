using System;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Classes {
	public class Cell 
	{
		public HeaderData Header { get; set; }
		public RowData Row { get; set; }
		public ObjectTypeInfo TypeInfo { get; set; }
		public string Data { get; set; }
		public object Tag { get; private set; }
		public string Name { get { return Header.Name; } }
		public int Width { get { return Header.Width; } }
		public int Height { get { return Row.Height; } }

		public bool IsEnum { get; set; }

		public Cell(HeaderData headerData, RowData row, ObjectTypeInfo typeInfo, string data, object underlyingData) 
		{
			Header = headerData;
			Row = row;
			TypeInfo = typeInfo;
			Data = data;
			Tag = underlyingData;
		}
	}
}