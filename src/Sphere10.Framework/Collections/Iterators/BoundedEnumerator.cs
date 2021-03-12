using System;
using System.Collections.Generic;

namespace Sphere10.Framework {


	public sealed class BoundedEnumerator<T> : EnumeratorDecorator<T> {
		private readonly int _maxCount;
		private int _count;
		
		public BoundedEnumerator(IEnumerator<T> enumerator, int maxCount)
			: base(enumerator) {
			_maxCount = maxCount;
			_count = 0;
		}

		public override bool MoveNext() => _count++ < _maxCount && base.MoveNext();

		public override void Reset() {
			base.Reset();
			_count = 0;
		}
	}
}