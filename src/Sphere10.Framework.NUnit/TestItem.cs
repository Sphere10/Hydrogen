using System;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework.NUnit {

	public class TestItem<TInput, TExpected> {
		public TInput Input { get; set; }
		public TExpected Expected { get; set; }
	}

	public class TestItem<TInput1, TInput2, TExpected> {
		public TInput1 Input1 { get; set; }
		public TInput2 Input2 { get; set; }
		public TExpected Expected { get; set; }
	}
	
}
