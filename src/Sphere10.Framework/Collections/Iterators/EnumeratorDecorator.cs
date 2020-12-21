using System;
using System.Collections;
using System.Collections.Generic;

namespace Sphere10.Framework {

	public abstract class EnumeratorDecorator<T> : IEnumerator<T> {
		private readonly IEnumerator<T> _enumerator;

		protected EnumeratorDecorator(IEnumerator<T> enumerator) {
			_enumerator = enumerator;
		}

		public virtual bool MoveNext() {
			return _enumerator.MoveNext();
		}

		public virtual void Reset() {
			_enumerator.Reset(); 
		}

		public virtual T Current => _enumerator.Current;

		object IEnumerator.Current => Current;

		public virtual void Dispose() {
			_enumerator.Dispose();
		}

	}

}