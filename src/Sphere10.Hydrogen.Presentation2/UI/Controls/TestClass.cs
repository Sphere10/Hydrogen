using System;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls {
	public class TestClass {

		public enum TestEnum {
			Black, White, Yellow, Purple, Brown 
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public TestEnum Color { get; set; }
		public DateTime CreationDate { get; set; }
		public decimal Age { get; set; }
		public string Details { get; set; }
		public string Note { get; set; }

		static string[] Names = { "Bitcoin", "Ethereum", "Polkadot", "Litecoin" };
		static TestEnum[] Colors = (TestEnum[])Enum.GetValues(typeof(TestEnum));

		public void FillWithTestData(int id) 
		{
			var seed = (int)DateTime.Now.Ticks;
			var random = new Random(seed);

			Id = id;
			Name = Names[random.Next(Names.Length - 1)];
			Color = Colors[random.Next(Colors.Length - 1)];
			CreationDate = new DateTime(random.Next(2010, 2021), random.Next(1, 13), 1);
			Age = random.Next(1, 100);
			Details = "Test Details";
			Note = "Test Note";
		}

		public override string ToString() 
		{
			return $"Id: {Id} Name: {Name} Color: {Color} CreationDate: {CreationDate.ToShortDateString()} Age: {Age} Details: {Details} Note: {Note}";
		}
	}
}