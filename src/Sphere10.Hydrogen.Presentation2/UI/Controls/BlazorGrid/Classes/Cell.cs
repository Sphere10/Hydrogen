using System;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Classes {
	public class Cell 
	{
		public HeaderData Header { get; set; }
		public RowData Row { get; set; }
		public Type DataType { get; set; }
		public string Data { get; set; }
		public object Tag { get; private set; }
		public string Name { get { return Header.Name; } }
		public int Width { get { return Header.Width; } }
		public int Height { get { return Row.Height; } }

		public Cell(HeaderData headerData, RowData row, Type dataType, string data, object underlyingData) 
		{
			Header = headerData;
			Row = row;
			DataType = dataType;
			Data = data;
			Tag = underlyingData;
		}
	}
}