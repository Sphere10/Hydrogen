//-----------------------------------------------------------------------
// <copyright file="GenericTableViewSource.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using Hydrogen;

namespace Hydrogen.iOS {

	// See this for adding improvements: https://gist.github.com/praeclarum/10024108
	public sealed class GenericTableViewSource<T> : UITableViewSource {
		private readonly ObservableList<GenericTableViewSection<T>> _sections;
		private readonly Func<T, GenericCellDescriptor> _itemDescriber;
		private readonly Action<T> _onSelectAction;
		private readonly string _cellIdentifier;
		private readonly UITableViewCellStyle _itemStyle;
		internal WeakReference<UITableView> _tableView;

		public GenericTableViewSource(
			UITableView tableView,
			UITableViewCellStyle itemStyle,
			IEnumerable<T> items,
			Func<T, GenericCellDescriptor> itemDescriber,
			Action<T> onSelectAction,
			string title = null
			) : this(
			tableView,
			itemStyle,
			new[] { new GenericTableViewSection<T>(title, items) },
			itemDescriber,
			onSelectAction
			) {
		}

		public GenericTableViewSource(
			UITableView tableView,
			UITableViewCellStyle itemStyle,
			IEnumerable<GenericTableViewSection<T>> sections,
			Func<T, GenericCellDescriptor> itemDescriber,
			Action<T> onSelectAction
		) : this(
			tableView,
			itemStyle,
			new ObservableList<GenericTableViewSection<T>>(sections.Where(s => s.Items.Any())),
			itemDescriber,
			onSelectAction
		) {
		}

		public GenericTableViewSource(
			UITableView tableView,
			UITableViewCellStyle itemStyle,
			ObservableList<GenericTableViewSection<T>> observableSections,
			Func<T, GenericCellDescriptor> itemDescriber,
			Action<T> onSelectAction
		) {
			if (tableView == null)
				throw new ArgumentNullException("tableView");

			if (observableSections == null)
				throw new ArgumentNullException("items");

			if (itemDescriber == null)
				throw new ArgumentNullException("displayNameGetter");

			if (onSelectAction == null)
				throw new ArgumentNullException("onSelectAction");

			_tableView = new WeakReference<UITableView>(tableView);
			_itemStyle = itemStyle;
			_sections = observableSections;
			_itemDescriber = itemDescriber;
			_onSelectAction = onSelectAction;
			_cellIdentifier = Guid.NewGuid().ToString();

			observableSections.ObservationWindow.Changed += HandleSectionsChanged;
		}



		public override nint NumberOfSections(UITableView tableView) {
			return (nint)_sections.Count;
		}

		public override string TitleForHeader(UITableView tableView, nint section) {
			return _sections[(int)section].Title;
		}

		public override nint RowsInSection(UITableView tableview, nint section) {
			return (nint)_sections[(int)section].Items.Count;
		}


		public override bool CanPerformAction(UITableView tableView, Selector action, NSIndexPath indexPath, NSObject sender) {
			return true;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath) {
			if (tableView == null) {
				throw new Exception("The datasource must have a reference to the table view for it to work");
			}
			var cell = tableView.DequeueReusableCell(_cellIdentifier) as GenericTableViewCell;
			// if there are no cells to reuse, create a new one
			if (cell == null) {
				cell = new GenericTableViewCell(_itemStyle, _cellIdentifier);
			} else {
				if (cell.ImageView != null) {
					if (cell.ImageView.Image != null) {
						cell.ImageView.Image.Dispose();
						cell.ImageView.Image = null;
					}
				}
			}
			var itemDescription = _itemDescriber(_sections[indexPath.Section].Items[indexPath.Row]);

			if (cell.TextLabel != null)
				cell.TextLabel.Text = itemDescription.Title ?? string.Empty;

			if (itemDescription.ImageGetter != null && cell.ImageView != null) {
				cell.ImageView.Image = itemDescription.ImageGetter();
				cell.ImageView.Frame = new CGRect(CGPoint.Empty, new CGSize(40, 40.0f));
				cell.ImageView.ContentMode = itemDescription.ImageContentMode;
				cell.ImageView.Layer.MasksToBounds = true;
			}

			if (cell.DetailTextLabel != null)
				cell.DetailTextLabel.Text = itemDescription.Description;

			cell.SelectionStyle = itemDescription.CanSelect ? UITableViewCellSelectionStyle.Default : UITableViewCellSelectionStyle.None;

			if (!string.IsNullOrWhiteSpace(itemDescription.BadgeText)) {
				cell.ShowBadgeView = true;
				cell.BadgeView.Text = itemDescription.BadgeText;

				if (itemDescription.BadgeBackgroundColor != null)
					cell.BadgeView.BackgroundColor = itemDescription.BadgeBackgroundColor;

				if (itemDescription.BadgeTextColor != null)
					cell.BadgeView.BackgroundColor = itemDescription.BadgeTextColor;

			} else {
				if (cell.AccessoryView is Hydrogen.iOS.BadgeView)
					cell.ShowBadgeView = false;
			}
			cell.Accessory = itemDescription.Accessory;

			if (itemDescription.CellConfig != null)
				itemDescription.CellConfig(cell, itemDescription);

			return cell;
		}

		public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath) {
			_onSelectAction(_sections[indexPath.Section].Items[indexPath.Row]);
		}

		public override Foundation.NSIndexPath WillSelectRow(UITableView tableView, Foundation.NSIndexPath indexPath) {
			return indexPath;
		}

		private void HandleSectionsChanged(ObservableList<GenericTableViewSection<T>> sectionsList, IEnumerable<ObservableCollectionItem<GenericTableViewSection<T>, int>> changes) {
			UITableView tableView;
			if (_tableView.TryGetTarget(out tableView)) {
				tableView.ReloadData();
			}
		}
	}
}

