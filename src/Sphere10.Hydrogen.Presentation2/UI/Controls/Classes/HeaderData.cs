using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.Classes 
{
	public class HeaderData
	{
		public string Name { get; set;}
		public int Width { get; set; }

		public HeaderData(string name, int width) 
		{
			Name = name;
			Width = width;
		}
	}
}
