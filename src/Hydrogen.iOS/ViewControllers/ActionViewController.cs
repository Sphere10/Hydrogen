//-----------------------------------------------------------------------
// <copyright file="ActionViewController.cs" company="Sphere 10 Software">
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
	public sealed class ActionViewController : UIViewController {
		private readonly UIView _userView;
		private readonly Func<UIInterfaceOrientation, bool> _shouldAutoRotate;
		private readonly Action<bool> _viewDidDisappear;
		private readonly Action<bool> _viewWillDisappear;

		public ActionViewController (UIView view, Action<bool> viewDidDisappear = null, Action<bool> viewWillDisappear = null, Func<UIInterfaceOrientation, bool> shouldAutoRotate = null) {
			_userView = view;
			_viewDidDisappear = viewDidDisappear;
			_viewWillDisappear = viewWillDisappear;
			_shouldAutoRotate = shouldAutoRotate;
		}

		public override void ViewDidLoad() {
			View.AddSubview(_userView);
		}

		public override void ViewDidDisappear (bool animated) {
			if (_viewDidDisappear != null) 
				_viewDidDisappear(animated);
		}

		public override void ViewWillDisappear (bool animated) {
			if (_viewWillDisappear != null) 
				_viewWillDisappear(animated);
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)	{
			if (_shouldAutoRotate != null) 
				return _shouldAutoRotate(toInterfaceOrientation);

			return base.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
		}
		
	}
}

