using System;
using System.IO;

namespace Sphere10.Framework {

	public sealed class OnDisposeStream : StreamDecorator {
		private readonly Action<Stream> _disposeAction;

		public OnDisposeStream(Stream stream, Action disposeAction)
			: this(stream, _ => disposeAction()) { 
		}

		public OnDisposeStream(Stream stream, Action<Stream> disposeAction)
			: base(stream) {
			_disposeAction = disposeAction;
		}

		protected override void Dispose(bool disposing) {
			_disposeAction?.Invoke(this);
			base.Dispose(disposing);
		}
	}

}