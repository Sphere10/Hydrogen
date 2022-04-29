//-----------------------------------------------------------------------
// <copyright file="MenuCollectionViewController.cs" company="Sphere 10 Software">
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
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Hydrogen.iOS {
	public class MenuCollectionViewController : UICollectionViewController {
		private static readonly NSString SiteCellID = new NSString("MenuCollectionViewController_00c91928-b351-477a-a3f9-8cad1e3b5b8a");
		private readonly IList<MenuOption> _menuOptions;
		private readonly Action<MenuOption> _selectAction;
		private NSIndexPath _selectdOption;

		public const float SectionInsetTop = 10;
		public const float SectionInsetLeft = 30;
		public const float SectionInsetRight = 0;
		public const float SectionInsetBottom = 10;
		public const float MenuItemWidth = 60;
		public const float MenuItemHeight =  87;
		public const float ImageWidthHeight = 60;
		public const float MenuItemHorizSpacing = 30;
		public const float MenuItemMinVerticalSpacing = 5;





		public MenuCollectionViewController(IList<MenuOption> options, Action<MenuOption> selectedAction) : this(DefaultLayout, options, selectedAction) {
		}

		public MenuCollectionViewController(UICollectionViewLayout layout, IList<MenuOption> options, Action<MenuOption> selectedAction) : base(layout) {
			_menuOptions = options;
			_selectAction = selectedAction;
			_selectdOption = null;
		}

		public static UICollectionViewLayout DefaultLayout {
			get {
				var flowLayout = new UICollectionViewFlowLayout {
					//HeaderReferenceSize = new System.Drawing.SizeF(100, 100),
					SectionInset = new UIEdgeInsets(SectionInsetTop, SectionInsetLeft, SectionInsetBottom, SectionInsetRight),
					ScrollDirection = UICollectionViewScrollDirection.Horizontal,
					ItemSize = new CGSize(MenuItemWidth, MenuItemHeight),


				};
				flowLayout.MinimumInteritemSpacing = MenuItemMinVerticalSpacing;
				flowLayout.MinimumLineSpacing = MenuItemHorizSpacing;

				return flowLayout;
			}
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {

			}
			base.Dispose(disposing);
		}

		public override void ViewDidLoad() {
			base.ViewDidLoad();
			CollectionView.RegisterClassForCell(typeof(MenuCell), SiteCellID);
			this.CollectionView.BackgroundColor = UIColor.Clear;
		}

		public override nint NumberOfSections(UICollectionView collectionView) {
			return 1;
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section) {
			return _menuOptions.Count;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath) {
			var menuOptionCell = (MenuCell)collectionView.DequeueReusableCell(SiteCellID, indexPath);
			var menuOption = _menuOptions[indexPath.Row];
			menuOptionCell.SetMenuOption(menuOption, _selectdOption != null && _selectdOption.Compare(indexPath) == 0);
			return menuOptionCell;
		}

		public override async void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath) {
			if (_selectAction != null) {
				var cell = collectionView.CellForItem(indexPath);
				//siteCell.ContentView.Subviews[0].Alpha = 0.5f;
				//	if (_selectdOption != null) {
				//		var previousSelection = collectionView.CellForItem(_selectdOption);
				//		if (previousSelection != null) {
				//			Animations.Fade(previousSelection.ContentView, 1.0f, TimeSpan.FromMilliseconds(100), null);
				//		}
				//	}
				//_selectdOption = indexPath;
				//siteCell.ContentView.Alpha = 0.5f;
				//Animations.Fade(siteCell.ContentView, 1.0f, TimeSpan.FromMilliseconds(300), null);
				cell.ContentView.Subviews[0].BackgroundColor = UIColor.Yellow;
				Animations.Fade(cell.ContentView, 0.25f, TimeSpan.FromMilliseconds(25), null);
				await Task.Delay(100);
				var site = _menuOptions[indexPath.Row];
				_selectAction(site);
			}
		}

		public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath) {
			var cell = collectionView.CellForItem(indexPath);
		}

		public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath) {
			var cell = collectionView.CellForItem(indexPath);
			//cell.ContentView.BackgroundColor = UIColor.Clear;
		}

		public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath) {
			return true;
		}

		public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath) {
			return true;
		}
		// for edit menu
		public override bool ShouldShowMenu(UICollectionView collectionView, NSIndexPath indexPath) {
			return false;
		}

		public override bool CanPerformAction(UICollectionView collectionView, ObjCRuntime.Selector action, NSIndexPath indexPath, NSObject sender) {
			return true;
		}


		public class MenuCell : UICollectionViewCell, ISpecialDisposable {
			UIImageView _imageView;
			UILabel _labelView;

			[Export("initWithFrame:")]
			public MenuCell(CGRect frame) : base(frame) {
				this.BackgroundColor = UIColor.Clear;
				BackgroundView = new UIView { BackgroundColor = UIColor.Clear };
				//SelectedBackgroundView = new UIView { BackgroundColor = UIColor.Red };

				//this.Layer.BorderColor = UIColor.Brown.CGColor;
				//this.Layer.BorderWidth = 1;

				ContentView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
				ContentView.Frame = ContentView.Bounds;
				//ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
				//ContentView.Layer.BorderWidth = 1.0f;
				ContentView.BackgroundColor = UIColor.Clear;
				//ContentView.Layer.CornerRadius = 5.0f;
				ContentView.Layer.MasksToBounds = true;

				_imageView = new UIImageView(/*UIImage.FromBundle("placeholder.png")*/);
				_imageView.Bounds = ContentView.Bounds;
				_imageView.Center = ContentView.Center;
				_imageView.ContentMode = UIViewContentMode.ScaleAspectFit;


				var imageContainerView = new UIView();
				imageContainerView.AddSubviewDockFull(_imageView, new UIEdgeInsets(5, 5, 5, 5));
				imageContainerView.BackgroundColor = UIColor.Clear;
				imageContainerView.Layer.BorderWidth = 2.0f;
				imageContainerView.Layer.BorderColor = this.TintColor.CGColor;
				imageContainerView.Layer.CornerRadius = 10.0f;


				ContentView.AddSubviewDock(imageContainerView, new UIEdgeInsets(0, 0, 0, 0), DockTarget.ToContainer(), DockTarget.ToContainer(), DockTarget.ToFixedLength(ImageWidthHeight), DockTarget.ToFixedLength(ImageWidthHeight/*+20*/));

				// create label
				_labelView = new UILabel(ContentView.Bounds);
				_labelView.TextColor = UIColor.Black;
				_labelView.BackgroundColor = UIColor.Clear;
				_labelView.Text = "SiteName";
				_labelView.Font = _labelView.Font.WithSize(10);
				_labelView.Lines = 2;
				_labelView.TextAlignment = UITextAlignment.Center;
				//_labelView.Layer.BorderWidth = 1;
				//_labelView.Layer.BorderColor = UIColor.Black.CGColor;

				ContentView.AddSubviewDock(_labelView, new UIEdgeInsets(3, 0, 0, 0), leftTarget: DockTarget.ToContainer(), topTarget: DockTarget.ToView(imageContainerView), rightTarget: DockTarget.ToContainer());
			}

			public void SetMenuOption(MenuOption option, bool asSelected) {
				ContentView.Alpha = asSelected ? 0.5f : 1.0f;
				//var imageFile = option.ImageFile;
				//UIImage oldImage = imageView.Image;
				//imageView.Image = UIImage.FromFile(imageFile);// AppCaches.GetImage(imageFile);
				_imageView.Image = option.Image;
				_labelView.Text = option.Title ?? string.Empty;
				//if (oldImage != null) {
				//	oldImage.Dispose();
				//}

			}


			public void SpecialDispose() {
				if (_imageView != null) {
					_imageView.DisposeEx();
				}
				if (_labelView != null) {
					_labelView.DisposeEx();
				}
				_imageView = null;
				_labelView = null;
			}


			protected override void Dispose(bool disposing) {
				if (disposing) {
					_imageView.DisposeEx();
					_labelView.DisposeEx();
					_imageView = null;
					_labelView = null;
				}
				base.Dispose(disposing);
			}
		}


	}
}
