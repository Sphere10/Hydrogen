//-----------------------------------------------------------------------
// <copyright file="UIPanGestureRecognizerEx.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using UIKit;

namespace Hydrogen.iOS {
	public class UIPanGestureRecognizerEx : UIPanGestureRecognizer {
		private Action<UIPanGestureRecognizerEx> _handler;
		private Token _token;

		public UIPanGestureRecognizerEx(Action<UIPanGestureRecognizerEx> handler) {
			if (handler == null)
				throw new ArgumentNullException("handler");

			this._handler = handler;
			_token = (this.AddTarget(() => _handler(this)));
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				this.RemoveTarget(_token);
				_token.Dispose();
				_token = null;
				_handler = null;
			}
		}
	}
}

