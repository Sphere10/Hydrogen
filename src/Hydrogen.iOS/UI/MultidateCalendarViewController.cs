//-----------------------------------------------------------------------
// <copyright file="MultidateCalendarViewController.cs" company="Sphere 10 Software">
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
using CoreGraphics;
using Foundation;
namespace Hydrogen.iOS
{
	public class Period {
		public DateTime DateFrom = DateTime.Now.AddDays(-7);
		public DateTime DateTo = DateTime.Now;
	}
	
	public enum Selecting {
		From,
		To
	}
	
	public class MultidateCalendarViewController : UIViewController
    {
		public MultidateCalendarViewController(){
			SelectedPeriod = new Period();	
		}
		
        public CalendarMonthView MonthView;
		public UITableView TableView;
		
		public Period SelectedPeriod {get;set;}
		
		private Selecting _selecting = Selecting.From;
		public Selecting Selecting {get {return _selecting; } set {_selecting = value; ChangedSelecting(); }}
		
		public void ChangedSelecting(){
			MonthView.DeselectDate();
			if (Selecting==Selecting.From){
				
				MonthView.OnFinishedDateSelection = (date) => {
					SelectedPeriod.DateFrom = date;
					SelectedPeriod.DateTo= date;
					UpdateData(TableView);
					MonthView.SetNeedsDisplay();
				};
				
				MonthView.IsDateAvailable = (date)=>{
					return (date <= DateTime.Today);
				};	
				
				MonthView.IsDayMarkedDelegate = (date) => {
					return (date>=SelectedPeriod.DateFrom && date <= SelectedPeriod.DateTo);
				};
			} else {
			
				MonthView.OnFinishedDateSelection = (date) => {
					SelectedPeriod.DateTo = date;
					UpdateData(TableView);
					MonthView.SetNeedsDisplay();
				};
				MonthView.IsDateAvailable = (date)=>{
					var available = (date <= DateTime.Today && date >= SelectedPeriod.DateFrom);
					return available;
				};
				MonthView.IsDayMarkedDelegate = (date) => {
					return (date>=SelectedPeriod.DateFrom && date <= SelectedPeriod.DateTo);
				};
			}
			if(TableView!=null)
				UpdateData(TableView);
			MonthView.SetNeedsDisplay();
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			MonthView.SetNeedsDisplay();
		}
        public override void ViewDidLoad()
        {
            MonthView = new CalendarMonthView();
			MonthView.Frame = new CoreGraphics.CGRect(new CGPoint(0,152), MonthView.Frame.Size);
			
			ChangedSelecting();
            View.AddSubview(MonthView);
			
			TableView = new UITableView(new CGRect(0,0,320,150), UITableViewStyle.Grouped);
			TableView.DataSource = new DateSource(this);
			TableView.Delegate = new DateDelegate(this);
			TableView.SelectRow((NSIndexPath)(NSIndexPath.FromRowSection(0,0)), true, UITableViewScrollPosition.Top);
			this.View.AddSubview(TableView);
        }
		
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return false;
        }
		
		public void UpdateData(UITableView tv){
			tv.CellAt((NSIndexPath)(NSIndexPath.FromRowSection(0,0))).DetailTextLabel.Text = SelectedPeriod.DateFrom.ToShortDateString();
			tv.CellAt((NSIndexPath)(NSIndexPath.FromRowSection(1,0))).DetailTextLabel.Text = SelectedPeriod.DateTo.ToShortDateString();
		}
	}
	
	class DateDelegate : UITableViewDelegate {
		MultidateCalendarViewController _dvc;
		public DateDelegate(MultidateCalendarViewController dvc){
			_dvc = dvc;
		}
		
		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			if (indexPath.Row==0){
				_dvc.Selecting = Selecting.From;
			} else if (indexPath.Row==1){
				_dvc.Selecting = Selecting.To;
			}
		}	
	}
	
	class DateSource : UITableViewDataSource {
		MultidateCalendarViewController _dvc;
		public DateSource(MultidateCalendarViewController dvc){
			_dvc = dvc;
		}
		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return (nint)2;
		}
		public override nint NumberOfSections (UITableView tableView)
		{
			return (nint)1;
		}
		
		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return "Select Period";
		}
		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell;
			if (indexPath.Row==0) {
				cell = new UITableViewCell(UITableViewCellStyle.Value1, "cell1");
				cell.TextLabel.Text = "From";
				cell.DetailTextLabel.Text =_dvc.SelectedPeriod.DateFrom.ToShortDateString();
				cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
				
			} else {
				cell = new UITableViewCell(UITableViewCellStyle.Value1, "cell2");
				cell.TextLabel.Text = "To";
				cell.DetailTextLabel.Text =_dvc.SelectedPeriod.DateTo.ToShortDateString();
				cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
			}
			return cell;
		}
		

	}
	
	
}
