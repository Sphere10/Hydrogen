using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Tests;

public class TestObject {

	public TestObject(Random random) {
		A = random.NextString(random.Next(0, 101));
		B = random.Next(0, 1000);
		C = random.NextBool();
	}

	public TestObject(string a, int b, bool c) {
		A = a;
		B = b;
		C = c;
	}

	public TestObject() {
	}

	public string A { get; set; }

	public int B { get; set; }

	public bool C { get; set; }

	public override string ToString() => $"[TestObject] A: '{A}', B: {B}, C: {C}";

}
