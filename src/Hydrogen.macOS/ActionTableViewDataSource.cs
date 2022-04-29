//-----------------------------------------------------------------------
// <copyright file="ActionTableViewDataSource.cs" company="Sphere 10 Software">
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
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Collections.Generic;

namespace Hydrogen {
	public sealed class ActionTableViewDataSource : NSTableViewDataSource {
		public ActionTableViewDataSource(
			 Func<NSTableView, NSDraggingInfo, int, NSTableViewDropOperation, bool> acceptDrop = null,
			Action<NSTableView, NSDraggingSession, PointF, NSDragOperation> draggingSessionEnded= null,
			Action<NSTableView, NSDraggingSession, PointF, NSIndexSet> draggingSessionWillBegin= null,
			Func<NSTableView, NSUrl, NSIndexSet, string[]> filesDropped= null,
			Func<NSTableView, NSTableColumn, int, NSObject>  getObjectValue= null,
			Func<NSTableView,int,NSPasteboardWriting> getPasteboardWriterForRow= null,
			Func<NSTableView,int> getRowCount= null,
			Action<NSTableView,NSObject,NSTableColumn, int> setObjectValue= null,
			Action<NSTableView, NSSortDescriptor[]> sortDescriptorsChanged= null,
			Action<NSTableView, NSDraggingInfo> updateDraggingItems= null,
			Func<NSTableView, NSDraggingInfo, int, NSTableViewDropOperation, NSDragOperation> validateDrop= null,
			Func<NSTableView, NSIndexSet, NSPasteboard, bool> writeRows= null) {

			AcceptDropFunc=acceptDrop;
			DraggingSessionEndedAction=draggingSessionEnded;
			DraggingSessionWillBeginAction=draggingSessionWillBegin;
			FilesDroppedFunc=filesDropped;
			GetObjectValueFunc=getObjectValue;
			GetPasteboardWriterForRowFunc=getPasteboardWriterForRow;
			GetRowCountFunc=getRowCount;
			SetObjectValueAction=setObjectValue;
			SortDescriptorsChangedAction=sortDescriptorsChanged;
			UpdateDraggingItemsAction=updateDraggingItems;
			ValidateDropFunc=validateDrop;
			WriteRowsFunc=writeRows;
		}

		public Func<NSTableView, NSDraggingInfo, int, NSTableViewDropOperation, bool> AcceptDropFunc { get; private set; }
		public Action<NSTableView, NSDraggingSession, PointF, NSDragOperation> DraggingSessionEndedAction { get; private set; }
		public Action<NSTableView, NSDraggingSession, PointF, NSIndexSet> DraggingSessionWillBeginAction { get; private set; }
		public Func<NSTableView, NSUrl, NSIndexSet, string[]> FilesDroppedFunc { get; private set; }
		public Func<NSTableView, NSTableColumn, int, NSObject>  GetObjectValueFunc { get; private set; }
		public Func<NSTableView,int,NSPasteboardWriting> GetPasteboardWriterForRowFunc  { get;	private set; }
		public Func<NSTableView,int> GetRowCountFunc  {	get; private set; }
		public Action<NSTableView,NSObject,NSTableColumn, int> SetObjectValueAction { get; private set; }
		public Action<NSTableView, NSSortDescriptor[]> SortDescriptorsChangedAction { get; private set; }
		public Action<NSTableView, NSDraggingInfo> UpdateDraggingItemsAction { get; private set; }
		public Func<NSTableView, NSDraggingInfo, int, NSTableViewDropOperation, NSDragOperation> ValidateDropFunc { get; private set; }
		public Func<NSTableView, NSIndexSet, NSPasteboard, bool> WriteRowsFunc { get; private set; }

		public override bool AcceptDrop(NSTableView tableView, NSDraggingInfo info, int row, NSTableViewDropOperation dropOperation) {
			if (AcceptDropFunc != null) {
				return AcceptDropFunc(tableView, info, row, dropOperation);
			}
			return default(bool);
		}

		public override void DraggingSessionEnded(NSTableView tableView, NSDraggingSession draggingSession, PointF endedAtScreenPoint, NSDragOperation operation) {
			if (DraggingSessionEndedAction != null) {
				DraggingSessionEndedAction(tableView, draggingSession, endedAtScreenPoint, operation);
			}
		}

		public override void DraggingSessionWillBegin(NSTableView tableView, NSDraggingSession draggingSession, PointF willBeginAtScreenPoint, NSIndexSet rowIndexes) {
			if (DraggingSessionWillBeginAction != null) {
				DraggingSessionWillBeginAction(tableView, draggingSession, willBeginAtScreenPoint, rowIndexes);
			}
		}

		public override string[] FilesDropped(NSTableView tableView, NSUrl dropDestination, NSIndexSet indexSet) {
			if (FilesDroppedFunc != null) {
				return FilesDroppedFunc(tableView, dropDestination, indexSet);
			}
			return default(string[]);
		}

		public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row) {
			if (GetObjectValueFunc != null) {
				return GetObjectValueFunc(tableView, tableColumn, row);
			}
			return default(NSObject);
		}



		public override NSPasteboardWriting GetPasteboardWriterForRow(NSTableView tableView, int row) {
			if (GetPasteboardWriterForRowFunc != null) {
				return GetPasteboardWriterForRowFunc(tableView, row);
			}
			return default(NSPasteboardWriting);
		}




		public override int GetRowCount(NSTableView tableView) {
			if (GetRowCountFunc != null) {
				return GetRowCountFunc(tableView);
			}
			return default(int);
		}



		public override void SetObjectValue(NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, int row) {
			if (SetObjectValueAction != null) {
				SetObjectValueAction(tableView, theObject, tableColumn, row);
			}
		}



		public override void SortDescriptorsChanged(NSTableView tableView, NSSortDescriptor[] oldDescriptors) {
			SortDescriptorsChangedAction(tableView, oldDescriptors);
		}




		public override void UpdateDraggingItems(NSTableView tableView, NSDraggingInfo draggingInfo) {
			if (UpdateDraggingItemsAction != null) {
				UpdateDraggingItemsAction(tableView, draggingInfo);
			}
		}



		public override NSDragOperation ValidateDrop(NSTableView tableView, NSDraggingInfo info, int row, NSTableViewDropOperation dropOperation) {
			if (ValidateDropFunc != null) {
				return ValidateDropFunc(tableView, info, row, dropOperation);
			}
			return default(NSDragOperation);
		}



		public override bool WriteRows(NSTableView tableView, NSIndexSet rowIndexes, NSPasteboard pboard) {
			if (WriteRowsFunc != null) {
				return WriteRowsFunc(tableView, rowIndexes, pboard);
			}
			return default(bool);
		}

		public IDictionary<int, object> PropertyBag {
			get;
			private set;
		}


	}
}

