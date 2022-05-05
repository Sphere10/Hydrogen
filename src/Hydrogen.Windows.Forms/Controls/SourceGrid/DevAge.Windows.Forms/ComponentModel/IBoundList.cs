//-----------------------------------------------------------------------
// <copyright file="IBoundList.cs" company="Sphere 10 Software">
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
namespace DevAge.ComponentModel
{
    /// <summary>
    /// A generic binded list interface. See BoundDataView or BoundList for a concreate implementation.
    /// It is used as an abstraction layer for list objects (typically a list can be a DataView or a IList class)
    /// Can be used to bind a list control (like SourceGrid).
    /// </summary>
    public interface IBoundList
    {
        bool AllowDelete { get; set; }
        bool AllowEdit { get; set; }
        bool AllowNew { get; set; }
        bool AllowSort { get; set; }
        void ApplySort(System.ComponentModel.ListSortDescriptionCollection sorts);
        int BeginAddNew();
        void BeginEdit(int index);
        int Count { get; }
        object EditedObject { get; }
        void EndEdit(bool cancel);
        System.ComponentModel.PropertyDescriptorCollection GetItemProperties();
        System.ComponentModel.PropertyDescriptor GetItemProperty(string name, StringComparison comparison);
        object GetItemValue(int index, System.ComponentModel.PropertyDescriptor property);
        int IndexOf(object item);
        event System.ComponentModel.ListChangedEventHandler ListChanged;
        void RemoveAt(int index);
        void SetEditValue(System.ComponentModel.PropertyDescriptor property, object value);
        object this[int index] { get; }
		
        event EventHandler ListCleared;
		event ItemDeletedEventHandler ItemDeleted;
	}

	/// <summary>
	/// Item deletion event handler.
	/// </summary>
	/// <param name="sender">Event sender.</param>
	/// <param name="e">Event arguments.</param>
	public delegate void ItemDeletedEventHandler ( object sender, ItemDeletedEventArgs e );

	/// <summary>
	/// Event arguments for item deletion event.
	/// </summary>
	public class ItemDeletedEventArgs : EventArgs
	{
		private object mItem;

		public object Item
		{
			get { return mItem; }
			set { mItem = value; }
		}

		public ItemDeletedEventArgs(object item)
		{
			mItem = item;
		}
	}
}
