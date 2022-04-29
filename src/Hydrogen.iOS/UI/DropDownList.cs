//-----------------------------------------------------------------------
// <copyright file="DropDownList.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Net.Mime;
using UIKit;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CoreGraphics;
using Foundation;
using Hydrogen;

namespace Hydrogen.iOS {

	// NOTE: refactor this to include editing, deleting, inserting, delay image loading and nicer interface

	// Interface improvements:
	//	- cell descriptor should include section
	//  - 

	// Implementation improvements
	// - use modal tool


	public static class DropDownList {
		private const float MinimumWidth = 320;
		private const float MaximumWidth = 320 * 2;
		private const float MinimumHeight = 100;
		private const float MaximumHeight = 500;
		private const float ItemPadding = 0;

		public static Task<DropDownListSelection<T>> Show<T>(
			UIView containerView,
			UIView anchorView,
			UITableViewCellStyle itemStyle,
			IEnumerable<T> items,
			Func<T, GenericCellDescriptor> itemDescriber,
			string dropDownTitle = null,
			UITableViewStyle headerStyle = UITableViewStyle.Plain,
			UIPopoverArrowDirection arrowDirection = UIPopoverArrowDirection.Up,
			CGSize? fitSize = null
		) {
			return ShowInternal(containerView, anchorView, itemStyle,headerStyle, items, itemDescriber, dropDownTitle, arrowDirection, fitSize);
		}

		public static Task<DropDownListSelection<T>> Show<T>(
			UITabBar containerView,
			UITabBarItem anchorView,
			UITableViewCellStyle itemStyle,
			IEnumerable<T> items,
			Func<T, GenericCellDescriptor> itemDescriber,
			string dropDownTitle = null,
			UITableViewStyle headerStyle = UITableViewStyle.Plain,
			UIPopoverArrowDirection arrowDirection = UIPopoverArrowDirection.Up,
			CGSize? fitSize = null
		) {
			return ShowInternal(containerView, anchorView, itemStyle,headerStyle, items, itemDescriber, dropDownTitle, arrowDirection, fitSize);
		}

		public static Task<DropDownListSelection<T>> Show<T>(
			UIBarButtonItem barButton,
			UITableViewCellStyle itemStyle,
			IEnumerable<T> items,
			Func<T, GenericCellDescriptor> itemDescriber,
			string dropDownTitle = null,
			UITableViewStyle headerStyle = UITableViewStyle.Plain,
			UIPopoverArrowDirection arrowDirection = UIPopoverArrowDirection.Up,
			CGSize? fitSize = null
		) {
			return ShowInternal(null, barButton, itemStyle,headerStyle, items, itemDescriber, dropDownTitle, arrowDirection, fitSize);
		}


		public static Task<DropDownListSelection<TItem>> Show<TSection, TItem>(
			UIView containerView,
			UIView anchorView,
			UITableViewCellStyle itemStyle,
			IEnumerable<TSection> sections,
			Func<TSection, GenericTableViewSection<TItem>> sectionDescriber,
			Func<TItem, GenericCellDescriptor> itemDescriber,
			UITableViewStyle headerStyle = UITableViewStyle.Plain,
			UIPopoverArrowDirection arrowDirection = UIPopoverArrowDirection.Up,
			CGSize? fitSize = null
		) where TSection : IEnumerable<TItem> {
			return ShowInternal(containerView, anchorView, itemStyle, headerStyle, sections, sectionDescriber, itemDescriber, arrowDirection, fitSize);
		}

		public static Task<DropDownListSelection<TItem>> Show<TSection, TItem>(
			UITabBar containerView,
			UITabBarItem anchorView,
			UITableViewCellStyle itemStyle,
			IEnumerable<TSection> sections,
			Func<TSection, GenericTableViewSection<TItem>> sectionDescriber,
			Func<TItem, GenericCellDescriptor> itemDescriber,
			UITableViewStyle headerStyle = UITableViewStyle.Plain,
			UIPopoverArrowDirection arrowDirection = UIPopoverArrowDirection.Up,
			CGSize? fitSize = null
			) where TSection : IEnumerable<TItem> {
			return ShowInternal(containerView, anchorView, itemStyle, headerStyle, sections, sectionDescriber, itemDescriber, arrowDirection, fitSize);
		}

		public static Task<DropDownListSelection<TItem>> Show<TSection, TItem>(
			UIBarButtonItem barButton,
			UITableViewCellStyle itemStyle,
			IEnumerable<TSection> sections,
			Func<TSection, GenericTableViewSection<TItem>> sectionDescriber,
			Func<TItem, GenericCellDescriptor> itemDescriber,
			UITableViewStyle headerStyle = UITableViewStyle.Plain,
			UIPopoverArrowDirection arrowDirection = UIPopoverArrowDirection.Up,
			CGSize? fitSize = null
			) where TSection : IEnumerable<TItem> {
			return ShowInternal(null, barButton, itemStyle, headerStyle, sections, sectionDescriber, itemDescriber, arrowDirection, fitSize);
		}


		#region Internal 

		private static Task<DropDownListSelection<T>> ShowInternal<T>(
			UIView containerView,
			object anchorUIElement,
			UITableViewCellStyle itemStyle,
			UITableViewStyle viewStyle,
			IEnumerable<T> items,
			Func<T, GenericCellDescriptor> itemDescriber,
			string dropDownTitle,
			UIPopoverArrowDirection arrowDirection,
			CGSize? fitSize
		) {
			if (itemDescriber == null)
				throw new ArgumentNullException("displayNameGetter");
				
			return ShowInternal(
				containerView,
				anchorUIElement,
				itemStyle,
				viewStyle,
				new [] { items },
				(s) => new GenericTableViewSection<T>(dropDownTitle, items),
				itemDescriber,
				arrowDirection,
				fitSize
			);
		}

		private static async Task<DropDownListSelection<TItem>> ShowInternal<TSection, TItem>(
			UIView containerView,
			object anchorUIElement,
			UITableViewCellStyle itemStyle,
			UITableViewStyle headerStyle,
			IEnumerable<TSection> sections,
			Func<TSection, GenericTableViewSection<TItem>> sectionDescriber,
			Func<TItem, GenericCellDescriptor> itemDescriber,
			UIPopoverArrowDirection arrowDirection,
			CGSize? fitSize
			) where TSection : IEnumerable<TItem> {
			if (anchorUIElement == null)
				throw new ArgumentNullException("anchorView");

			if (!(anchorUIElement is UIView || anchorUIElement is UIBarButtonItem)) {
				throw new ArgumentOutOfRangeException("anchorView", "Must be a UIView or a UIBarButtonItem");
			}

			var sectionsArray = sections.Select(sectionDescriber).ToArray();


			if (sectionsArray.Count() == 1 && string.IsNullOrWhiteSpace(sectionsArray.First().Title))
				headerStyle = UITableViewStyle.Plain;

			var selectSignal = new ManualResetEventSlim();
			var table = new UITableViewController(headerStyle);
			table.AutomaticallyAdjustsScrollViewInsets = false;
			var result = new DropDownListSelection<TItem>();
			var popOverController = new UIPopoverController(table);

			var dropDownSource = new GenericTableViewSource<TItem>(
				table.TableView,
				itemStyle,
				sectionsArray,
				itemDescriber,
				(x) => {
					result.SelectedItem = x;
					popOverController.Dismiss(false);
					selectSignal.Set();
				}
			);
			popOverController.DidDismiss += (object sender, EventArgs e) => {
				if (!selectSignal.IsSet)
					selectSignal.Set();
			};

			((UITableView)table.View).Source = dropDownSource;

			// Calculate how wide the view should be by finding how 
			if (fitSize == null) {
				// calculate the fit size
				var tableView = (UITableView)table.View;
				nfloat largestWidth = 0.0f;
                var sectionHeight = (headerStyle == UITableViewStyle.Plain ? (float)tableView.SectionHeaderHeight : Tools.iOSTool.DefaultGroupedTableViewHeaderHeight);
                nfloat totalHeight = sectionHeight * sectionsArray.Count(s => !string.IsNullOrWhiteSpace(s.Title));
				foreach (var section in sectionsArray) {
					foreach (var itemDescriptor in ((IEnumerable<TItem>)section.Items).WithDescriptions()) {
						var index = itemDescriptor.Index;
						var item = itemDescriptor.Description;
						var cellSize = (CGSize)dropDownSource.GetCell(tableView, (NSIndexPath)(NSIndexPath.FromRowSection(index, 0))).SizeThatFits((CGSize)CGSize.Empty);
						if (cellSize.Width > largestWidth)
							largestWidth = cellSize.Width;
						totalHeight += cellSize.Height;
					}
				}

				fitSize = new CGSize(largestWidth + ItemPadding, totalHeight);
			}
			popOverController.SetPopoverContentSize(fitSize.Value, false);

			TypeSwitch.Do(
				anchorUIElement,
				TypeSwitch.Case<UIBarButtonItem>( bbi => 
					popOverController.PresentFromBarButtonItem(
						(UIBarButtonItem)anchorUIElement,
						arrowDirection,
						true
					)
				),
				TypeSwitch.Case<UIView>(v => 
					popOverController.PresentFromRect(
						((UIView)anchorUIElement).Frame,
						containerView,
						arrowDirection,
						true
					)
				),
				TypeSwitch.Case<UITabBarItem>( tb => {
					var tabBar = containerView as UITabBar;
					if (tabBar == null)
						throw new ArgumentException("Container view must be an UITabBar when the anchor view is an UITabBarItem");

					if (!tabBar.Items.Contains(tb))
						throw new ArgumentException("Container view UITabBar does not contain the anchor view UITabBarItem");

					var index = tabBar.Items.IndexOf(tb);
					var tabBarItemWidth = tabBar.Frame.Size.Width / tabBar.Items.Count();
					var rect = new CGRect(tabBarItemWidth * index, 0f, tabBarItemWidth, tabBar.Frame.Height);

					popOverController.PresentFromRect(
						rect,
						tabBar,
						arrowDirection,
						true
					);
				}),
				TypeSwitch.Default( () => { throw new NotSupportedException("anchorUIElement was {0}".FormatWith(anchorUIElement.GetType().Name)); })
			);

			await Task.Run(() => selectSignal.Wait());
			table.View.DisposeEx();
			table.TableView.DisposeEx();
			table.Dispose();
			popOverController.Dispose();

			return result;
		}

		#endregion

	}
}

