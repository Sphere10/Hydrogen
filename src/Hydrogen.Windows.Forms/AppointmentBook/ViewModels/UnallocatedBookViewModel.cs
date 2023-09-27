// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Windows.Forms.AppointmentBook;

public class UnallocatedBookViewModel : AppointmentBookViewModel {
	private const string DefaultUnallocatedColumnName = "Unallocated";

	ColumnViewModel _unallocatedColumn;
	readonly IDictionary<int, CellViewModel> _rowToCellLookup;

	public UnallocatedBookViewModel(string columnName, IEnumerable<AppointmentViewModel> unallocatedBlocks, DateTime timePeriodStart, TimePeriodType timeView)
		: base(new[] { new ColumnViewModel(columnName, unallocatedBlocks.ToArray(), new UnallocatedColumn()) }, timePeriodStart, timeView) {
		_rowToCellLookup = new Dictionary<int, CellViewModel>();
		_unallocatedColumn = base.GetColumnAt(0);
		RecreateCellDisplays();
	}

	protected override void SetVisibleColumns() {
		try {
			base.SetVisibleColumns();
			RecreateCellDisplays();
		} catch (Exception error) {
			SystemLog.Exception(error);
			ExceptionDialog.Show(error);
		}
	}

	public virtual ColumnViewModel UnallocatedColumn {
		get { return _unallocatedColumn; }
		set {
			_unallocatedColumn = value;
			// "Render" the model 
			RecreateCellDisplays();
		}
	}

	internal override int GetColumnCount() {
		return 1;
	}

	internal override int GetRowCount() {
		// start counting with first empty row
		int count = 1;

		// count all unallocated block lengths plus their separator row
		_unallocatedColumn.Appointments.ForEach(a => count += a.Lines.Length + 1);

		return Tools.Values.ClipValue(count, base.FinishTimeToRow(base.TimePeriodEnd), count);
	}


	protected override void RecreateCellDisplays() {
		if (_unallocatedColumn != null) {
			_blockLookup.Clear();
			_rowToCellLookup.Clear();
			var endRow = -1;
			_unallocatedColumn.Index = 0;
			var unseenBlocks = new List<AppointmentViewModel>();
			for (var i = 0; i < _unallocatedColumn.Appointments.Length; i++) {
				var appointmentBlock = _unallocatedColumn.Appointments[i];

				if (appointmentBlock.AppointmentDataObject.EndTime <= TimePeriodStart || appointmentBlock.AppointmentDataObject.StartTime >= TimePeriodEnd) {
					unseenBlocks.Add(appointmentBlock);
					continue;
				}

				appointmentBlock.VisibleStartTime = appointmentBlock.AppointmentDataObject.StartTime;
				appointmentBlock.VisibleEndTime = appointmentBlock.AppointmentDataObject.EndTime;

				appointmentBlock.Column = _unallocatedColumn;
				var startRow = endRow + 2;
				endRow = startRow + (FinishTimeToRow(appointmentBlock.VisibleEndTime) - StartTimeToRow(appointmentBlock.VisibleStartTime));
				/*if (TimeView == TimeView.Monthly) {
					endRow++; // since a block is a full day, we say it finishes on next row (wont actually draw on it)
				}*/
				appointmentBlock.StartRow = startRow;
				appointmentBlock.EndRow = endRow;
				appointmentBlock.Lines = CellViewModel.CreateArray(endRow - startRow + 1);

				// Update the lookup
				for (int j = startRow; j <= endRow; j++) {
					_blockLookup[Tuple.Create(0, j)] = appointmentBlock;
					_rowToCellLookup[j] = appointmentBlock.Lines[j - startRow];
				}

				// Set the traits of the cells
				appointmentBlock.Lines.WithDescriptions().ForEach(cellDescription => {
					cellDescription.Item.Traits = 0;
					if (cellDescription.Description.HasFlag(EnumeratedItemDescription.First)) {
						cellDescription.Item.Traits = cellDescription.Item.Traits.CopyAndSetFlags(CellTraits.Top);
					}

					if (cellDescription.Description.HasFlag(EnumeratedItemDescription.Last)) {
						cellDescription.Item.Traits = cellDescription.Item.Traits.CopyAndSetFlags(CellTraits.Bottom);
					}

					if (cellDescription.Description.HasFlag(EnumeratedItemDescription.Interior)) {
						cellDescription.Item.Traits = cellDescription.Item.Traits.CopyAndSetFlags(CellTraits.Interior);
					}
				});
				appointmentBlock.RequestRender(appointmentBlock, appointmentBlock.Lines);
			}
			_unallocatedColumn.Appointments = _unallocatedColumn.Appointments.Except(unseenBlocks).ToArray();
		}

	}

	internal override CellViewModel GetCellDisplay(int col, int row) {
		var cellViewModel = _rowToCellLookup.ContainsKey(row) ? _rowToCellLookup[row] : CellViewModel.Empty;
		return cellViewModel;
	}


}
