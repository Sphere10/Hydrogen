using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.Classes {
	public class Cell 
	{
		public HeaderData Header { get; set; }
		public RowData Row { get; set; }
		public Type DataType { get; set; }
		public string Data { get; set; }
		public string Name { get { return Header.Name; } }
		public int Width { get { return Header.Width; } }
		public int Height { get { return Row.Height; } }

		public Cell(HeaderData headerData, RowData row, Type dataType, string data) 
		{
			Header = headerData;
			Row = row;
			DataType = dataType;
			Data = data;
		}
	}
}
