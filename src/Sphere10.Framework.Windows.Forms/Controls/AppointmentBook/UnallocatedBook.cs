//-----------------------------------------------------------------------
// <copyright file="UnallocatedBook.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SourceGrid;

namespace Sphere10.Framework.Windows.Forms.AppointmentBook {

	[ToolboxItem(true)]
	public class UnallocatedBook : AppointmentBook {

		public UnallocatedBook() : base(new DataSourceToUnallocatedViewModelConverter()) {
			HasRowHeaders = false;
			_grid.AutoStretchColumnsToFitWidth = true;
		}

		public override bool CanResize {
			get {
				return false;
			}
			set {
				base.CanResize = value;
			}
		}

		public new virtual UnallocatedBookViewModel ViewModel {
			get {
				return base.ViewModel as UnallocatedBookViewModel;
			}
			set {
				base.ViewModel = value as UnallocatedBookViewModel; // base will bind
			}
		}

		protected override void BindToViewModel() {
			base.BindToViewModel();
			if (ViewModel != null) {
				for (int i = 0; i < ViewModel.GetRowCount(); i++)
					base.RedrawCell(0, i);
			}
		}

		protected override bool IsTimeAvailable(AppointmentColumn column, Appointment appointment, DateTime startTime, DateTime endTime) {
			return true;
		}

		public override bool CanSelect(int col, int row) {
			return false;
		}

		public override bool CanDragCell(int col, int row) {
			return ViewModel.GetAppointmentBlockAt(col, row) != null;
		}

		protected override bool OnAppointmentDropStarting(AppointmentColumn originalColumn, AppointmentColumn targetColumn, Appointment appointment, DateTime startTime, DateTime endTime) {
			return true;
		}
	}
}
