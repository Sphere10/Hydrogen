//-----------------------------------------------------------------------
// <copyright file="ApplicationScreenViewController.cs" company="Sphere 10 Software">
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
using ObjCRuntime;
using System.Threading.Tasks;

namespace Hydrogen.iOS {

	// TODO needs work
public class ApplicationScreenViewController : KeyboardAwareViewController {
		public event EventHandlerEx Loaded;
		public event EventHandlerEx WillAppear;
		public event EventHandlerEx Appeared;
		public event EventHandlerEx WillDisappear;
		public event EventHandlerEx Disappeared;
		public event EventHandlerEx BackTapped;
		public event EventHandlerEx MenuTapped;
		public event EventHandlerEx NextTapped;
		public event EventHandlerEx Dismissed;
		public event EventHandlerEx Disposing;
		public bool SuppressNotification;
		private readonly bool _hasBackButton;
		private readonly bool _hasMenuButton;
		private readonly bool _useBackgroundTexture;
		protected List<IDisposable> Disposables;
		private Throttle _backThrottle = new Throttle(1);

		public ApplicationScreenViewController() 
			:this(null, null, false, false, false, false) {

		}
		public ApplicationScreenViewController(string nibName, NSBundle bundle, bool tapEndsEditing, bool hasBackButton, bool hasMenuButton, bool useBackgroundTexture): base(nibName, bundle, tapEndsEditing) {
			_hasBackButton = hasBackButton;
			_hasMenuButton = hasMenuButton;
			_useBackgroundTexture = useBackgroundTexture;
			SuppressNotification = false;
			Disposables = new List<IDisposable>();

		}


		public new string Title { get { return this.NavigationItem.Title; } set { this.NavigationItem.Title = value; } }

		public override void ViewDidLoad() {

			base.ViewDidLoad();

			// Perform any additional setup after loading the view, typically from a nib.

			// 1. Custom back button (culture neutral)
			this.NavigationItem.HidesBackButton = !_hasBackButton;
			if (_hasBackButton) {
				this.NavigationItem.LeftBarButtonItem = CreateBackButton();
			}

			if (_hasMenuButton) {
				this.NavigationItem.RightBarButtonItem = CreateMenuButton();
			}

			// 2. Apply background texture if applicable
			if (_useBackgroundTexture) {
				this.View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("smooth_wall"));
			}

			// 3. Notify presenter view has been loaded
			FireLoaded();
		}

		protected async void HandleMenuTapped() {
			try {
				throw new NotImplementedException();
			} catch (Exception error) {
				//await this.ShowException(error);
			}
		}


		public override void ViewWillAppear(bool animated) {
			base.ViewWillAppear(animated);
			FireWillAppear();
		}

		public override void ViewDidAppear(bool animated) {
			base.ViewDidAppear(animated);
			FireAppeared();
		}


		public override void ViewWillDisappear(bool animated) {
			base.ViewWillDisappear(animated);
			FireWillDisappear();
		}

		public override void ViewDidDisappear(bool animated) {
			base.ViewDidDisappear(animated);
			FireDisappeared();
		}

		public override void DismissViewController(bool animated, Action completionHandler) {
			base.DismissViewController(
				animated,
				() => {
					completionHandler();

					FireDismissed();
				});
		}

		protected virtual async Task<bool> CanGoBack() {
			return true;
		}

		protected virtual async Task<bool> CanShowMenu() {
			return true;
		}

		protected UIBarButtonItem CreateBackButton() {
			return new UIBarButtonItem(
				"\U000025C0\U0000FE0E",
				UIBarButtonItemStyle.Plain,
				async (sender, e) => {
					if (await CanGoBack())
						FireBackTapped();
				}
			);
		}

		protected UIBarButtonItem CreateMenuButton() {
			return new UIBarButtonItem(
			   UIImage.FromBundle("TODO"),
			   UIBarButtonItemStyle.Bordered,
(EventHandler)((o, e) =>
			   Tools.Exceptions.ExecuteIgnoringException((Action)(async () => {
				   if (await CanShowMenu())
					   MenuTapped?.Invoke();
			   }))));
		}


		protected void DisposeDisposables() {
			Disposables.ForEach(d => d.Dispose());
			Disposables.Clear();
		}

		private void FireLoaded() {
			if (Loaded != null)
				Loaded();
		}

		private void FireWillAppear() {
			if (WillAppear != null)
				WillAppear();
		}

		private void FireAppeared() {
			if (Appeared != null)
				Appeared();
		}

		private void FireWillDisappear() {
			if (WillDisappear != null) {
				WillDisappear();
			}
		}

		private void FireDisappeared() {
			if (Disappeared != null)
				Disappeared();
		}

		private void FireBackTapped() {
			if (!_backThrottle.CanPass())
				return;

			if (BackTapped != null)
				BackTapped();
		}

		private void FireDismissed() {
			if (!SuppressNotification) {
				if (Dismissed != null)
					Dismissed();
			}
		}

		private void NotifyDisposing() {
			if (Disposing != null)
				Disposing();
		}

		public virtual void ReleaseEventHandlers() {
			Loaded = null;
			WillAppear = null;
			Appeared = null;
			WillDisappear = null;
			Disappeared = null;
			BackTapped = null;
			MenuTapped = null;
			MenuTapped = null;
			Dismissed = null;
			Disposing = null;
			if (_hasBackButton) {
				var btn = this.NavigationItem.LeftBarButtonItem;
				NavigationItem.LeftBarButtonItem = null;
				if (btn != null) {
					btn.Dispose();
				}
			}
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				NotifyDisposing();
				DisposeDisposables();
				View.DisposeEx();
			}
			base.Dispose(disposing);
		}


	}
}


