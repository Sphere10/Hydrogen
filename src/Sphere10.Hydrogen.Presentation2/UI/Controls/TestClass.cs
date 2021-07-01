using System;
using System.Linq;

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



		public void FillWithTestData(int id) {
			var random = new Random(666);

			Id = id;
			Name = Names[random.Next(Names.Length - 1)];
			Color = Colors[random.Next(Colors.Length - 1)];

		}
	}
}