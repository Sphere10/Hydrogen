namespace Hydrogen {

	public class DebugLogger : TextWriterLogger {
		public DebugLogger()
			: base(new DebugTextWriter()) {
		}
	}

}
