//-----------------------------------------------------------------------
// <copyright file="ControlsTableViewController.cs" company="Sphere 10 Software">
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
using System.Linq;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Hydrogen.iOS
{
		[Foundation.Register("ControlsTableViewController")]
		public partial class ControlsTableViewController : UITableViewController {
			static NSString kCellIdentifier = new NSString ("MyIdentifier");
		
			public List<string> _controls = new List<string>();	
		
			//
			// Constructor invoked from the NIB loader
			//
			public ControlsTableViewController (IntPtr p) : base (p) {}
		
			
			//
			// The data source for our TableView
			//
			class DataSource : UITableViewDataSource {
				ControlsTableViewController tvc;
			
				public DataSource (ControlsTableViewController tvc)
				{
					this.tvc = tvc;
					tvc._controls.Add("PagedViewController");
					tvc._controls.Add("CalendarMonthView");
					tvc._controls.Add("MultidateCalendarViewController");
					tvc._controls.Add("LoadingHUDView");
					tvc._controls.Add("ImageListView");
					tvc._controls.Add("UIWebImageView");
					tvc._controls.Add("RotatingViewController");
					tvc._controls.Add("UIDecimalField");
				}
				
				public override nint RowsInSection (UITableView tableView, nint section)
				{
					return (nint)tvc._controls.Count();
				}
		
				public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
				{
					var cell = tableView.DequeueReusableCell (kCellIdentifier);
					if (cell == null){
						cell = new UITableViewCell (UITableViewCellStyle.Default, kCellIdentifier);
					}
				
					cell.TextLabel.Text = tvc._controls[indexPath.Row];
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					return cell;
				}
			}
		
			//
			// This class receives notifications that happen on the UITableView
			//
			class TableDelegate : UITableViewDelegate {
				ControlsTableViewController tvc;
	
				public TableDelegate (ControlsTableViewController tvc)
				{
					this.tvc = tvc;
				}
				
				public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
				{
					var selected = tvc._controls[indexPath.Row];
				
					Console.WriteLine ("escoz: Row selected {0}", selected);
				
					switch (selected){
						case "PagedViewController":
							tvc.NavigationController.PushViewController(new PagedViewController{
								PagedViewDataSource = new TestPagesDataSource()}, true);
							break;
						case "UIWebImageView":
							tvc.NavigationController.PushViewController(new UIWebImageViewController(), true);
							break;
					
						case "RotatingViewController":
							tvc.NavigationController.PushViewController(_createRotatingViewController(), true);
							break;
						
						case "UIDecimalField":
							tvc.NavigationController.PushViewController(
					                 new AmountEditorViewController(1234.23m, "Amount"), true);
							break;
					
						case "LoadingHUDView" :
							tvc.NavigationController.PushViewController(
					                 new LoadingHUDViewController(), true);
							break;
						case "CalendarMonthView" :
							tvc.NavigationController.PushViewController(
					                 new CalendarMonthViewController{Title="Calendar"}, true);
							break;
						case "MultidateCalendarViewController" :
							tvc.NavigationController.PushViewController(
					                 new MultidateCalendarViewController{Title="Period"}, true);
							break;
						case "ImageListView" :
							tvc.NavigationController.PushViewController(
					                 _createImageListViewController(), true);
							break;
					}
				}
			
				public UIViewController _createImageListViewController(){
					var controller = new ImageListViewController{
						Title="Images"
					};
					return controller;
				}
			
				public UIViewController _createRotatingViewController(){
					var rvc = new RotatingViewController();
					rvc.PortraitViewController = new UIViewController{View = new UIView{ BackgroundColor = UIColor.Blue }};
					rvc.LandscapeLeftViewController = new TestViewController{View = new UIView {BackgroundColor = UIColor.Green}};
					rvc.LandscapeRightViewController = new UIWebImageViewController(); 
					rvc.PortraitViewController.View.AddSubview(new UILabel {
									Text = "PortraitView. Rotate for more.", 
									Frame = new CoreGraphics.CGRect(10,10,300,40), 
									BackgroundColor = UIColor.Blue});
				
					rvc.LandscapeLeftViewController.View.AddSubview(new UILabel {
									Text = "LandscapeLeftView.", 
									Frame = new CoreGraphics.CGRect(10,10,300,40), 
									BackgroundColor = UIColor.Green});
					return rvc;
				}
			}
			
			public override void ViewDidLoad ()
			{
				base.ViewDidLoad ();
				Title = "Controls";
				
				TableView.Delegate = new TableDelegate (this);
				TableView.DataSource = new DataSource (this);
			}
			
	}
	
	public class TestViewController : UIViewController {
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		public override bool WantsFullScreenLayout {
			get {
				return true;
			}
			set {
				base.WantsFullScreenLayout = value;
			}
		}

	}
	
	
	public class TestPagesDataSource : IPagedViewDataSource {
			
		private List<UIColor> _colors = new List<UIColor>{UIColor.Blue, UIColor.Red, UIColor.Yellow, UIColor.Cyan};
		
		public UIViewController GetPage(int i){
			UIViewController c = new UIViewController();
			c.View.BackgroundColor = _colors[i];
			c.View.Frame = new CGRect(10,10,300,200);
			c.View.AddSubview(new UILabel{
				Text= "Swipe to change view", 
				TextAlignment = UITextAlignment.Center,
				Frame = new CGRect(0,0,320,100),
				BackgroundColor = UIColor.Clear,
			});
			return c;
		}
		
		public void Reload(){}
		
		public int Pages {get {return 4;} }
	}
		
}
