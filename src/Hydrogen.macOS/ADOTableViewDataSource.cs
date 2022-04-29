//-----------------------------------------------------------------------
// <copyright file="ADOTableViewDataSource.cs" company="Sphere 10 Software">
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
using System.Data;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Hydrogen;
using System.Drawing;
using Hydrogen;


namespace Hydrogen.Data {

	public class ADOTableViewDataSource : NSTableViewDataSource {


		public ADOTableViewDataSource(IntPtr handle) : base (handle) {
			Initialize();
		}

		public ADOTableViewDataSource(DataTable dataTable = null) {
			Initialize(dataTable);
		}

		private void Initialize(DataTable dataTable = null) {
			DataTable = dataTable;
		}

		public DataTable DataTable { get; set; }

		public override bool AcceptDrop(NSTableView tableView, NSDraggingInfo info, int row, NSTableViewDropOperation dropOperation) {
			return false;
		}
		
		public override void DraggingSessionEnded(NSTableView tableView, NSDraggingSession draggingSession, PointF endedAtScreenPoint, NSDragOperation operation) {

		}
		
		public override void DraggingSessionWillBegin(NSTableView tableView, NSDraggingSession draggingSession, PointF willBeginAtScreenPoint, NSIndexSet rowIndexes) {

		}
		
		public override string[] FilesDropped(NSTableView tableView, NSUrl dropDestination, NSIndexSet indexSet) {
			return new string[0];
		}
		
		public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row) {
			var col = tableColumn.GetIndex();
			return DataTable.Rows[row][col].ToNSObject();
		}
		
		public override NSPasteboardWriting GetPasteboardWriterForRow(NSTableView tableView, int row) {
			return null;
		}

		public override int GetRowCount(NSTableView tableView) {
			return DataTable.Rows.Count;
		}

		public override void SetObjectValue(NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, int row) {
			var col = tableColumn.GetIndex();
			DataTable.Rows[row][col] = theObject.FromNSObject<object>();
		}

		public override void SortDescriptorsChanged(NSTableView tableView, NSSortDescriptor[] oldDescriptors) {

		}

		public override void UpdateDraggingItems(NSTableView tableView, NSDraggingInfo draggingInfo) {

		}

		public override NSDragOperation ValidateDrop(NSTableView tableView, NSDraggingInfo info, int row, NSTableViewDropOperation dropOperation) {
			return NSDragOperation.None;
		}

		public override bool WriteRows(NSTableView tableView, NSIndexSet rowIndexes, NSPasteboard pboard) {
			return false;
		}

	


	}
}

