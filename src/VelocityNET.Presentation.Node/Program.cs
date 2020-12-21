using Sphere10.Framework;
using Terminal.Gui;

namespace VelocityNET.Presentation.Node {

	class Program {

		static void Main() {
			SystemLog.RegisterLogger(new TimestampLogger(new DebugLogger()));
			Navigator.Start();
		}
	}

}