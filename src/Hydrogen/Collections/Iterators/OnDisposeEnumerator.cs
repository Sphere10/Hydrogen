using System;
using System.Collections.Generic;

namespace Hydrogen {

	public sealed class OnDisposeEnumerator<T> : EnumeratorDecorator<T> {
		private readonly Action _disposeAction;

		public OnDisposeEnumerator(IEnumerator<T> enumerator, Action disposeAction) 
			: base(enumerator) {
			_disposeAction = disposeAction;
		}

		public override void Dispose() {
			_disposeAction?.Invoke();
			base.Dispose();	
		}
	}

}