using Sphere10.Framework;
using Terminal.Gui;
using Sphere10.Hydrogen.Node.UI;

namespace Sphere10.Hydrogen.Node {

	class Program {

		static void Main() {
			SystemLog.RegisterLogger(new TimestampLogger(new DebugLogger()));
			Navigator.Start();
		}
	}

}