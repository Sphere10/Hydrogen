//-----------------------------------------------------------------------
// <copyright file="KeyboardAwareViewController.cs" company="Sphere 10 Software">
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
using Foundation;
using CoreGraphics;
using Hydrogen;
using System.Collections.Generic;

namespace Hydrogen.iOS {
	public class KeyboardAwareViewController : UIViewController {
		private const float KeyboardPadding = 10.0f;    // extra amount to scroll
		private UIView _focusedView;            // Controller that activated the keyboard  
		private nfloat _scrollDistance = 0.0f;  // amount to scroll
		private nfloat _bottomY = 0.0f;         // bottom point
		private bool _moveViewUp = false;       // which direction are we moving
		private NSObject _popupObserver;
		private NSObject _popdownObserver;
		private nfloat _keyboardHeight;
		private IDictionary<UIView, Func<bool>> _returnActions;
		private UITapGestureRecognizer _tapGesture;
		private bool _rotating = false;

		public KeyboardAwareViewController(string nibName, NSBundle bundle, bool tapEndsEditing) : base(nibName, bundle) {
			Initialize(tapEndsEditing);
		}

		public bool TapEndsEditing { get; private set; }

		private void Initialize(bool tapEndsEditing) {
			_returnActions = new Dictionary<UIView, Func<bool>>();
			TapEndsEditing = tapEndsEditing;
			_tapGesture = new UITapGestureRecognizer();
			_tapGesture.AddTarget(() => {
				TapEndsEditingContainerView.EndEditing(true);
			});
		}

		protected virtual UIView TapEndsEditingContainerView => View;

		public override void ViewDidLoad() {
			base.ViewDidLoad();

			// Make sure keyboard goes away when finished
			if (TapEndsEditing)
				TapEndsEditingContainerView.AddGestureRecognizer(_tapGesture);

		}

		public override void ViewWillAppear(bool animated) {
			base.ViewWillAppear(animated);
			// Keyboard popup
			_popupObserver = NSNotificationCenter.DefaultCenter.AddObserver
				(UIKeyboard.WillShowNotification, KeyboardUpNotification);

			// Keyboard Down
			_popdownObserver = NSNotificationCenter.DefaultCenter.AddObserver
				(UIKeyboard.WillHideNotification, KeyboardDownNotification);

		}

		public override void ViewWillDisappear(bool animated) {
			if (_popupObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(_popupObserver);

			if (_popdownObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(_popdownObserver);
		}

		public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration) {
			_rotating = true;
			base.WillRotate(toInterfaceOrientation, duration);
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation) {
			base.DidRotate(fromInterfaceOrientation);
			_rotating = false;
		}

		private void KeyboardUpNotification(NSNotification notification) {
			// get the keyboard size
			_keyboardHeight = Tools.iOSTool.GetKeyboardHeight(this.View, notification);
			ScrollFirstResponderAboveKeyboard();
		}

		private void KeyboardDownNotification(NSNotification notification) {

			if (_moveViewUp) {
				ScrollTheView(false);
			}

			_keyboardHeight = 0;
		}

		public void ScrollFirstResponderAboveKeyboard() {
			_focusedView = null;
			// Find what opened the keyboard
			foreach (UIView view in this.View.Subviews) {
				if (view.IsFirstResponder) {
					_focusedView = view;
					break;
				}
			}

			if (_focusedView == null)
				return;

			// Bottom of the controller = initial position + height + offset      
			if (!(View is UIScrollView))
				throw new SoftwareException("Must be a scroll view");

			//this.View.ConvertPointToCoordinateSpace(_focusedView.Frame.Location, UICoordinateSpace.
			var statusBarHeight = UIApplication.SharedApplication?.StatusBarFrame.Height ?? (nfloat)0.0f;
			var topBarHeight = (nfloat)0.0f;
			var frameHeight = View.Frame.Size.Height;
			var botBarHeight = (nfloat)0.0f;
			if (this.NavigationController != null && this.NavigationController.NavigationBar.Translucent) {
				topBarHeight = this.NavigationController.NavigationBar.Frame.Size.Height;
				frameHeight = View.Frame.Size.Height - statusBarHeight - topBarHeight - botBarHeight;
			}

			_bottomY = _focusedView.Frame.Y + _focusedView.Frame.Height + KeyboardPadding;
			var keyboardY = frameHeight - _keyboardHeight;

			// Calculate how far we need to scroll
			//var newScrollAmount = View is UIScrollView ? _keyboardHeight - _bottomY : (_keyboardHeight - (View.Frame.Size.Height - _bottomY));
			var newScrollAmount = keyboardY - _bottomY;

			_scrollDistance = newScrollAmount - _scrollDistance;

			// Perform the scrolling
			if (_scrollDistance < 0) {
				_moveViewUp = true;
				ScrollTheView(_moveViewUp);
			} else {
				_moveViewUp = false;
			}
		}

		public void WhenReturnFocusOn(UITextField textField, UITextField nextTextField) {
			WhenReturn(
				textField,
				() => {
					nextTextField.BecomeFirstResponder();
					//ScrollFirstResponderAboveKeyboard();
					return true;
				}
			);
		}

		public void WhenReturnDismissKeyboard(UITextField textField) {
			WhenReturn(
				textField,
				() => {
					textField.ResignFirstResponder();
					return true;
				}
			);
		}

		public void WhenReturnDismissKeyboardAnd(UITextField textField, Action action) {
			WhenReturn(
				textField,
				 () => {
					 textField.ResignFirstResponder();
					 //await Task.Delay(20);
					 action();
					 return true;
				 }
			);
		}

		public void WhenReturn(UITextField textField, Func<bool> action) {
			textField.WeakDelegate = this;
			_returnActions[textField] = action;
		}

		[Export("textFieldShouldReturn:")]
		public virtual bool ShouldReturn(UITextField textField) {
			if (_returnActions.ContainsKey(textField)) {
				var action = _returnActions[textField];
				return action();
			}

			textField.ResignFirstResponder();
			return true;
		}

		private nfloat? _frameBaseY = null;
		private void ScrollTheView(bool move) {
			if (_frameBaseY == null)
				_frameBaseY = View.Frame.Y;

			// scroll the view up or down
			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration(0.3);

			CGRect frame = View.Frame;

			if (move) {
				frame.Y += _scrollDistance;
			} else {
				frame.Y = _frameBaseY.Value;
				_scrollDistance = 0;
			}

			View.Frame = frame;
			UIView.CommitAnimations();
		}
	}
}

