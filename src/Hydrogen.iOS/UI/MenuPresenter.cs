//-----------------------------------------------------------------------
// <copyright file="MenuPresenter.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using Hydrogen;
using System.Collections.Generic;
using System.Threading;
using CoreGraphics;

namespace Hydrogen.iOS {
	public static class MenuPresenter {

		

		static public async Task<MenuOption> ShowContextMenu(UIView anchorView, IEnumerable<MenuOption> menuOptions) {
			var selectSignal = new ManualResetEventSlim();

			var options = menuOptions.ToArray();
			// create menu (segmented control)
			var segmentedControl = new UISegmentedControl(options.Select(o => o.Title).ToArray());

			var cta = segmentedControl.GetTitleTextAttributes(UIControlState.Normal);

			var ta = new UITextAttributes();
			ta.Font = cta.Font;
			ta.TextColor = UIColor.White;
			ta.TextShadowColor = UIColor.White;
			ta.TextShadowOffset = cta.TextShadowOffset;

			segmentedControl.TintColor = UIColor.Clear;
			segmentedControl.SetTitleTextAttributes(ta, UIControlState.Normal);

			var controller = new UIViewController();
			controller.ModalPresentationStyle = UIModalPresentationStyle.Popover;
			controller.View = segmentedControl;
			segmentedControl.Layer.BorderWidth = 0;


			int selectedIndex = -1;
			segmentedControl.ValueChanged += (object sender, EventArgs e) => {
				selectedIndex = (int)segmentedControl.SelectedSegment;
				controller.DismissViewController(true, selectSignal.Set);
			};
			var fitSize = segmentedControl.SizeThatFits(CGSize.Empty);
			controller.PreferredContentSize = fitSize;
			var topMost = Tools.iOSTool.GetTopMostController();

			controller.PopoverPresentationController.BackgroundColor = UIColor.Black;
			controller.PopoverPresentationController.SourceView = anchorView;
			controller.PopoverPresentationController.SourceRect = anchorView.Bounds;
			controller.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Any;

			controller.PopoverPresentationController.Delegate = new ActionPopoverPresentationDelegate(
				() => {
					if (!selectSignal.IsSet)
						selectSignal.Set();
				},
				(arg) => UIModalPresentationStyle.None
			);
			topMost.PresentViewController(controller, true, null);
			await Task.Run(() => selectSignal.Wait());
			var result = selectedIndex != -1 ? options[selectedIndex] : null;
			return result;
		}

		static public async Task<MenuOption> ShowMenu(UIView anchorView, IEnumerable<MenuOption> menuOptions) {
			var selectSignal = new ManualResetEventSlim();
			int selectedIndex = -1;
			var options = menuOptions.ToArray();
			var topMost = Tools.iOSTool.GetTopMostController();
			var controller = new UIViewController();

			// create menu (segmented control)
			var menuController = new MenuCollectionViewController(menuOptions.ToList(), (x) => Tools.Exceptions.ExecuteIgnoringException(() => {
				selectedIndex = options.IndexOf(x);
				controller.DismissViewController(true, selectSignal.Set);
			}));

			menuController.CollectionView.Layer.BorderWidth = 0;
			var fitSize = new CGSize(topMost.View.Frame.Width, 100 * 3);
			controller.PreferredContentSize = fitSize;
			controller.View.Bounds = new CGRect(CGPoint.Empty, fitSize);
			controller.ModalPresentationStyle = UIModalPresentationStyle.Popover;
			controller.View = menuController.CollectionView;
			controller.PopoverPresentationController.BackgroundColor = UIColor.FromWhiteAlpha(0.95f, 0.9f);
			controller.PopoverPresentationController.SourceView = anchorView;
			controller.PopoverPresentationController.SourceRect = new CGRect(anchorView.Bounds.X, anchorView.Bounds.Y + 180, anchorView.Bounds.Width, anchorView.Bounds.Height);
			controller.PopoverPresentationController.PermittedArrowDirections = (UIPopoverArrowDirection)0;

			controller.PopoverPresentationController.Delegate = new ActionPopoverPresentationDelegate(
				() => {
					if (!selectSignal.IsSet)
						selectSignal.Set();
				},
				(arg) => UIModalPresentationStyle.None
			);

			topMost.PresentViewController(controller, true, null);
			await Task.Run(() => selectSignal.Wait());
			var result = selectedIndex != -1 ? options[selectedIndex] : null;
			return result;
		}



		private class ActionPopoverPresentationDelegate : UIPopoverPresentationControllerDelegate {
			private readonly Action _didDismissPopoverAction;
			private readonly Func<UIPresentationController, UIModalPresentationStyle> _getAdaptivePresentationStyle;

			public ActionPopoverPresentationDelegate(Action didDismissPopoverAction = null, Func<UIPresentationController, UIModalPresentationStyle> getAdaptivePresentationStyle = null) {
				_didDismissPopoverAction = didDismissPopoverAction ?? (() => Tools.Lambda.NoOp());
				_getAdaptivePresentationStyle = getAdaptivePresentationStyle ?? ((x) => base.GetAdaptivePresentationStyle(x));
			}


			public override void DidDismissPopover(UIPopoverPresentationController popoverPresentationController) {
				_didDismissPopoverAction();
			}

			public override UIModalPresentationStyle GetAdaptivePresentationStyle(UIPresentationController forPresentationController) {
				return _getAdaptivePresentationStyle(forPresentationController);
			}

		}
	}
}

