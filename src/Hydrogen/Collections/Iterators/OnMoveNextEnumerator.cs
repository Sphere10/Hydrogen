using System;
using System.Collections.Generic;

namespace Hydrogen {

	public sealed class OnMoveNextEnumerator<T> : EnumeratorDecorator<T> {
		private readonly Func<bool?> _preMoveNextFunc;
		private readonly Func<bool, bool?> _postMoveNextFunc;

		public OnMoveNextEnumerator(IEnumerator<T> enumerator, Action preMoveNextAction)
			: this(enumerator, preMoveNextAction, null) {
		}

		public OnMoveNextEnumerator(IEnumerator<T> enumerator, Action preMoveNextAction, Action<bool> postMoveNextAction)
			: this(
				enumerator, 
				() => {
					preMoveNextAction?.Invoke();
					return null; // this means don't intercept call
				},
				moveNextResult => {
					postMoveNextAction?.Invoke(moveNextResult);
					return null; // this means don't intercept call
				}) {
		}

		public OnMoveNextEnumerator(IEnumerator<T> enumerator, Func<bool?> preMoveNextFunc, Func<bool, bool?> postMoveNextFunc) 
			: base(enumerator) {
			_preMoveNextFunc = preMoveNextFunc ?? (()=>null);
			_postMoveNextFunc = postMoveNextFunc ?? (_=>null);
		}

		public override bool MoveNext() {
			var intercept = _preMoveNextFunc();
			var result = intercept ?? base.MoveNext();
			intercept = _postMoveNextFunc(result);
			if (intercept.HasValue)
				result = intercept.Value;
			return result;
		}

	}

}