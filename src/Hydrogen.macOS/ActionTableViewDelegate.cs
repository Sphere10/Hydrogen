//-----------------------------------------------------------------------
// <copyright file="ActionTableViewDelegate.cs" company="Sphere 10 Software">
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
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Hydrogen {
	public class ActionTableViewDelegate : NSTableViewDelegate {

		public ActionTableViewDelegate(
			Func<NSTableView, NSEvent, string, bool> shouldTypeSelectFunc = null,
			Action<NSTableView, NSObject, NSTableColumn, int> willDisplayCellAction = null,
			Action<NSNotification> columnDidMoveAction = null,
			Action<NSNotification> columnDidResizeAction = null,
			Func<NSTableView, int, NSTableRowView> coreGetRowViewFunc = null,
			Action<NSTableView, NSTableRowView, int> didAddRowViewAction = null,
			Action<NSTableView, NSTableColumn> didClickTableColumnAction = null,
			Action<NSTableView, NSTableColumn> didDragTableColumnAction = null,
			Action<NSTableView, NSTableRowView, int> didRemoveRowViewAction = null,
			Func<NSTableView, NSTableColumn, int, NSCell> getDataCellFunc = null,
			Func<NSTableView, int, int, string, int> getNextTypeSelectMatchFunc = null,
			Func<NSTableView, int, float> getRowHeightFunc = null,
			Func<NSTableView, NSIndexSet, NSIndexSet> getSelectionIndexesFunc = null,
			Func<NSTableView, NSTableColumn, int,string> getSelectStringFunc = null,
			Func<NSTableView, int, float> getSizeToFitColumnWidthFunc = null,
			Func<NSTableView, NSTableColumn, int, NSView> getViewForItemFunc = null,
			Func<NSTableView, int, bool> isGroupRowFunc = null,
			Action<NSTableView, NSTableColumn> mouseDownAction = null,
			Action<NSNotification> selectionDidChangeAction = null,
			Action<NSNotification> selectionIsChangingAction = null,
			Func<NSTableView, bool> selectionShouldChangeFunc = null,
			Func<NSTableView, NSTableColumn, int, bool> shouldEditTableColumnFunc = null,
			Func<NSTableView, int, int, bool> shouldReorderFunc = null,
			Func<NSTableView, int, bool> shouldSelectRowFunc = null,
			Func<NSTableView, NSTableColumn, bool> shouldSelectTableColumnFunc = null,
			Func<NSTableView, NSTableColumn, int, bool> shouldShowCellExpansionFunc = null,
			Func<NSTableView, NSCell, NSTableColumn, int, bool> shouldTrackCellFunc  = null
		) {
			ShouldTypeSelectFunc = shouldTypeSelectFunc;
			WillDisplayCellAction = willDisplayCellAction;
			ColumnDidMoveAction = columnDidMoveAction;
			ColumnDidResizeAction = columnDidResizeAction;
			CoreGetRowViewFunc = coreGetRowViewFunc;
			DidAddRowViewAction = didAddRowViewAction;
			DidClickTableColumnAction = didClickTableColumnAction;
			DidDragTableColumnAction = didDragTableColumnAction;
			DidRemoveRowViewAction = didRemoveRowViewAction;
			GetDataCellFunc = getDataCellFunc;
			GetNextTypeSelectMatchFunc = getNextTypeSelectMatchFunc;
			GetRowHeightFunc = getRowHeightFunc;
			GetSelectionIndexesFunc = getSelectionIndexesFunc;
			GetSelectStringFunc = getSelectStringFunc;
			GetSizeToFitColumnWidthFunc = getSizeToFitColumnWidthFunc;
			GetViewForItemFunc = getViewForItemFunc;
			IsGroupRowFunc = isGroupRowFunc;
			MouseDownAction = mouseDownAction;
			SelectionDidChangeAction = selectionDidChangeAction;
			SelectionIsChangingAction = selectionIsChangingAction;
			SelectionShouldChangeFunc = selectionShouldChangeFunc;
			ShouldEditTableColumnFunc = shouldEditTableColumnFunc;
			ShouldReorderFunc = shouldReorderFunc;
			ShouldSelectRowFunc = shouldSelectRowFunc;
			ShouldSelectTableColumnFunc = shouldSelectTableColumnFunc;
			ShouldShowCellExpansionFunc = shouldShowCellExpansionFunc;
			ShouldTrackCellFunc = shouldTrackCellFunc;
		}

		private Func<NSTableView, NSEvent, string, bool> ShouldTypeSelectFunc { get; set; }

		private Action<NSTableView, NSObject, NSTableColumn, int> WillDisplayCellAction { get; set; }

		private Action<NSNotification> ColumnDidMoveAction { get; set; }

		private Action<NSNotification> ColumnDidResizeAction { get; set; }

		private Func<NSTableView, int, NSTableRowView> CoreGetRowViewFunc { get; set; }

		private Action<NSTableView, NSTableRowView , int> DidAddRowViewAction { get; set; }

		private Action<NSTableView, NSTableColumn> DidClickTableColumnAction { get; set; }

		private Action<NSTableView, NSTableColumn> DidDragTableColumnAction { get; set; }

		private Action<NSTableView, NSTableRowView, int> DidRemoveRowViewAction { get; set; }

		private Func<NSTableView, NSTableColumn, int, NSCell> GetDataCellFunc { get; set; }

		private Func<NSTableView, int, int, string, int> GetNextTypeSelectMatchFunc { get; set; }

		private Func<NSTableView, int, float> GetRowHeightFunc { get; set; }

		private Func<NSTableView, NSIndexSet, NSIndexSet> GetSelectionIndexesFunc { get; set; }

		private Func<NSTableView, NSTableColumn, int,string> GetSelectStringFunc { get; set; }

		private Func<NSTableView, int, float> GetSizeToFitColumnWidthFunc { get; set; }

		private Func<NSTableView, NSTableColumn, int, NSView> GetViewForItemFunc { get; set; }

		private Func<NSTableView, int, bool> IsGroupRowFunc { get; set; }

		private Action<NSTableView, NSTableColumn> MouseDownAction { get; set; }

		private Action<NSNotification> SelectionDidChangeAction { get; set; }

		private Action<NSNotification> SelectionIsChangingAction { get; set; }

		private Func<NSTableView, bool> SelectionShouldChangeFunc { get; set; }

		private Func<NSTableView, NSTableColumn, int, bool> ShouldEditTableColumnFunc { get; set; }

		private Func<NSTableView, int, int, bool> ShouldReorderFunc { get; set; }

		private Func<NSTableView, int, bool> ShouldSelectRowFunc { get; set; }

		private Func<NSTableView, NSTableColumn, bool> ShouldSelectTableColumnFunc { get; set; }

		private Func<NSTableView, NSTableColumn, int, bool> ShouldShowCellExpansionFunc { get; set; }

		private Func<NSTableView, NSCell, NSTableColumn, int, bool> ShouldTrackCellFunc { get; set; }

		public override void ColumnDidMove(NSNotification notification) {
			if (ColumnDidMoveAction != null) {
				ColumnDidMoveAction(notification);
			}
		}

		public override void ColumnDidResize(NSNotification notification) {
			if (ColumnDidResizeAction != null) {
				ColumnDidResizeAction(notification);
			}
		}

		public virtual NSTableRowView CoreGetRowView(NSTableView tableView, int row) {
			if (CoreGetRowViewFunc != null) {
				return CoreGetRowViewFunc(tableView, row);
			}
			return default(NSTableRowView);
		}

		public override void DidAddRowView(NSTableView tableView, NSTableRowView rowView, int row) {
			if (DidAddRowViewAction != null) {
				DidAddRowViewAction(tableView, rowView, row);
			}
		}

		public override void DidClickTableColumn(NSTableView tableView, NSTableColumn tableColumn) {
			if (DidClickTableColumnAction != null) {
				DidClickTableColumnAction(tableView, tableColumn);
			}
		}

		public override void DidDragTableColumn(NSTableView tableView, NSTableColumn tableColumn) {
			if (DidDragTableColumnAction != null) {
				DidDragTableColumnAction(tableView, tableColumn);
			}
		}

		public override void DidRemoveRowView(NSTableView tableView, NSTableRowView rowView, int row) {
			if (DidRemoveRowViewAction != null) {
				DidRemoveRowViewAction(tableView, rowView, row);
			}
		}

		public override NSCell GetDataCell(NSTableView tableView, NSTableColumn tableColumn, int row) {
			if (GetDataCellFunc != null) {
				return GetDataCellFunc(tableView, tableColumn, row);
			}
			return default(NSCell);
		}

		public override int GetNextTypeSelectMatch(NSTableView tableView, int startRow, int endRow, string searchString) {
			if (GetNextTypeSelectMatchFunc != null) {
				return GetNextTypeSelectMatchFunc(tableView, startRow, endRow, searchString);
			}
			return default(int);
		}

		public override float GetRowHeight(NSTableView tableView, int row) {
			if (GetRowHeightFunc != null) {
				return GetRowHeightFunc(tableView, row);
			}
			return default(float);
		}

		public override NSIndexSet GetSelectionIndexes(NSTableView tableView, NSIndexSet proposedSelectionIndexes) {
			if (GetSelectionIndexesFunc != null) {
				return GetSelectionIndexesFunc(tableView, proposedSelectionIndexes);
			}
			return default(NSIndexSet);		
		}

		public override string GetSelectString(NSTableView tableView, NSTableColumn tableColumn, int row) {
			if (GetSelectStringFunc != null) {
				return GetSelectStringFunc(tableView, tableColumn, row);
			}
			return default(string);
		}

		public override float GetSizeToFitColumnWidth(NSTableView tableView, int column) {
			if (GetSizeToFitColumnWidthFunc != null) {
				return GetSizeToFitColumnWidthFunc(tableView, column);
			}
			return default(float);
		}

		public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row) {
			if (GetViewForItemFunc != null) {
				return GetViewForItemFunc(tableView, tableColumn, row);
			}
			return default(NSView);
		}

		public override bool IsGroupRow(NSTableView tableView, int row) {
			if (IsGroupRowFunc != null) {
				return IsGroupRowFunc(tableView, row);
			}
			return default(bool);
		}

		public override void MouseDownInHeaderOfTableColumn (NSTableView tableView, NSTableColumn tableColumn)
		{
			if (MouseDownAction != null) {
				MouseDownAction(tableView, tableColumn);
			}
		}

		public override void SelectionDidChange(NSNotification notification) {
			if (SelectionDidChangeAction != null) {
				SelectionDidChangeAction(notification);
			}
		}

		public override void SelectionIsChanging(NSNotification notification) {
			if (SelectionIsChangingAction != null) {
				SelectionIsChangingAction(notification);
			}
		}

		public override bool SelectionShouldChange(NSTableView tableView) {
			if (SelectionShouldChangeFunc != null) {
				return SelectionShouldChangeFunc(tableView);
			}
			return default(bool);
		}

		public override bool ShouldEditTableColumn(NSTableView tableView, NSTableColumn tableColumn, int row) {
			if (ShouldEditTableColumnFunc != null) {
				return ShouldEditTableColumnFunc(tableView, tableColumn, row);
			}
			return default(bool);
		}

		public override bool ShouldReorder(NSTableView tableView, int columnIndex, int newColumnIndex) {
			if (ShouldReorderFunc != null) {
				return ShouldReorderFunc(tableView, columnIndex, newColumnIndex);
			}
			return default(bool);
		}

		public override bool ShouldSelectRow(NSTableView tableView, int row) {
			if (ShouldSelectRowFunc != null) {
				return ShouldSelectRowFunc(tableView, row);
			}
			return default(bool);
		}

		public override bool ShouldSelectTableColumn(NSTableView tableView, NSTableColumn tableColumn) {
			if (ShouldSelectTableColumnFunc != null) {
				return ShouldSelectTableColumnFunc(tableView, tableColumn);
			}
			return default(bool);
		}

		public override bool ShouldShowCellExpansion(NSTableView tableView, NSTableColumn tableColumn, int row) {
			if (ShouldShowCellExpansionFunc != null) {
				return ShouldShowCellExpansionFunc(tableView, tableColumn, row);
			}
			return default(bool);
		}

		public override bool ShouldTrackCell(NSTableView tableView, NSCell cell, NSTableColumn tableColumn, int row) {
			if (ShouldTrackCellFunc != null) {
				return ShouldTrackCellFunc(tableView, cell, tableColumn, row);
			}
			return default(bool);
		}
			
		public override bool ShouldTypeSelect(NSTableView tableView, NSEvent theEvent, string searchString) {
			if (ShouldTypeSelectFunc != null) {
				return ShouldTypeSelectFunc(tableView, theEvent, searchString);
			}
			return default(bool);
		}

		public override void WillDisplayCell(NSTableView tableView, NSObject cell, NSTableColumn tableColumn, int row) {
			if (WillDisplayCellAction != null) {
				WillDisplayCellAction(tableView, cell, tableColumn, row);
			}
		}
			
	}

}

