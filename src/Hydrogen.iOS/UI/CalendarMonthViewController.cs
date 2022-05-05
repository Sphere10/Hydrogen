//-----------------------------------------------------------------------
// <copyright file="CalendarMonthViewController.cs" company="Sphere 10 Software">
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
using Foundation;
using UIKit;
using System.Collections;
using System.Collections.Generic;
using CoreGraphics;
namespace Hydrogen.iOS
{
	public class CalendarMonthViewController : UIViewController
    {

        public CalendarMonthView MonthView;

        public override void ViewDidLoad()
        {
            MonthView = new CalendarMonthView();
			MonthView.OnDateSelected += (date) => {
				Console.WriteLine(String.Format("Selected {0}", date.ToShortDateString()));
			};
			MonthView.OnFinishedDateSelection = (date) => {
				Console.WriteLine(String.Format("Finished selecting {0}", date.ToShortDateString()));
			};
			MonthView.IsDayMarkedDelegate = (date) => {
				return (date.Day % 2==0) ? true : false;
			};
			MonthView.IsDateAvailable = (date)=>{
				return (date > DateTime.Today);
			};
			
            View.AddSubview(MonthView);
        }
		
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return false;
        }

    }


}
