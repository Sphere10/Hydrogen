namespace Sphere10.Framework {

	public class DebugLogger : TextWriterLogger {
		public DebugLogger()
			: base(new DebugTextWriter()) {
		}
	}

}
