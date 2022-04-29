﻿using System;

namespace Hydrogen.DApp.Presentation2.UI.Controls {
	public class TestClass {

		public enum TestEnum 
		{
			Black, White, Yellow, Purple, Brown , Blue
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

		static Random Random { get; set; } = new Random((int)DateTime.Now.Ticks);

		public void FillWithTestData(int id) 
		{
			Id = id;
			Name = Names[Random.Next(Names.Length - 1)];
			Color = Colors[Random.Next(Colors.Length - 1)];
			CreationDate = new DateTime(Random.Next(2010, 2021), Random.Next(1, 13), 1);
			Age = Random.Next(1, 100);
			Details = "Test Details";
			Note = "Test Note";
		}

		public override string ToString() 
		{
			return $"Id: {Id} Name: {Name} Color: {Color} CreationDate: {CreationDate.ToShortDateString()} Age: {Age} Details: {Details} Note: {Note}";
		}
	}
}