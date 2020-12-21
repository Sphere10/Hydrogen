using System;
using System.Collections.Generic;

namespace Sphere10.Framework {

	public sealed class OnMoveNextEnumerator<T> : EnumeratorDecorator<T> {
		private readonly Action _moveNextAction;

		public OnMoveNextEnumerator(IEnumerator<T> enumerator, Action moveNextAction) 
			: base(enumerator) {
			_moveNextAction = moveNextAction;
		}

		public override bool MoveNext() {
			_moveNextAction();
			return base.MoveNext();
		}

	}

}