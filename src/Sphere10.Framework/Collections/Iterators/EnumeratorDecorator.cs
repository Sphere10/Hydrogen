using System.Collections;
using System.Collections.Generic;

namespace Sphere10.Framework {

	public class EnumeratorDecorator<TFrom, TTo> : IEnumerator<TTo> where TTo : TFrom {

		public EnumeratorDecorator(IEnumerator<TFrom> enumerator) {
			InternalEumerator = enumerator;
		}

		protected readonly IEnumerator<TFrom> InternalEumerator;

		public virtual bool MoveNext() {
			return InternalEumerator.MoveNext();
		}

		public virtual void Reset() {
			InternalEumerator.Reset(); 
		}

		public virtual TTo Current => (TTo)InternalEumerator.Current;

		object IEnumerator.Current => Current;

		public virtual void Dispose() {
			InternalEumerator.Dispose();
		}

	}

	public class EnumeratorDecorator<T> : EnumeratorDecorator<T,T> {
		protected EnumeratorDecorator(IEnumerator<T> enumerator) : base(enumerator) {
		}
	}

}