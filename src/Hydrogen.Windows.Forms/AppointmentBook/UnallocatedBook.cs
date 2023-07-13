// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;

namespace Hydrogen.Windows.Forms.AppointmentBook;

[ToolboxItem(true)]
public class UnallocatedBook : AppointmentBook {

	public UnallocatedBook() : base(new DataSourceToUnallocatedViewModelConverter()) {
		HasRowHeaders = false;
		_grid.AutoStretchColumnsToFitWidth = true;
	}

	public override bool CanResize {
		get { return false; }
		set { base.CanResize = value; }
	}

	public new virtual UnallocatedBookViewModel ViewModel {
		get { return base.ViewModel as UnallocatedBookViewModel; }
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
