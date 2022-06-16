namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes {
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