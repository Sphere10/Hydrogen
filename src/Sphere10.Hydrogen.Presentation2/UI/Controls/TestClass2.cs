namespace Sphere10.Hydrogen.Presentation2.UI.Controls {
	public class TestClass2 
	{
		public string Text { get; set; }
		public decimal Value { get; set; }
		public bool Locked { get; set; }

		public TestClass2(string text, decimal value, bool locked) 
		{
			Text = text;
			Value = value;
			Locked = locked;
		}
	}
}