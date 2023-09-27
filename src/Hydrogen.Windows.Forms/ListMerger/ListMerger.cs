// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;

namespace Hydrogen.Windows.Forms;

public delegate void ItemSelectedHandler(ListMerger source, ListMergerSide side, object selectedItem);


public delegate void ItemsMovedHandler(ListMerger source, ListMergerSide from, ListMergerSide to, ListMergeMode action, IEnumerable<object> item);


public partial class ListMerger : UserControl {

	public ListMerger() {
		InitializeComponent();
		DisplayMember = ValueMember = null;
		MergeMode = ListMergeMode.Move;
	}

	#region Events

	public event ItemSelectedHandler ItemSelected;
	public event ItemsMovedHandler ItemsMoved;

	#endregion

	#region Properties

	[Category("Behavior")]
	[DefaultValue(ListMergeMode.Move)]
	public ListMergeMode MergeMode { get; set; }

	[Category("Behavior")]
	public string DisplayMember {
		get { return _leftListBox.DisplayMember; }
		set {
			_leftListBox.DisplayMember = value;
			_rightListBox.DisplayMember = value;
		}
	}

	[Category("Behavior")]
	public string ValueMember {
		get { return _leftListBox.ValueMember; }
		set {
			_leftListBox.ValueMember = value;
			_rightListBox.ValueMember = value;
		}
	}

	[Category("Appearance")]
	[DefaultValue("Left Header")]
	public string LeftHeader {
		get { return _leftHeaderLabel.Text; }
		set { _leftHeaderLabel.Text = value; }
	}

	[Category("Appearance")]
	[DefaultValue("Right Header")]
	public string RightHeader {
		get { return _rightHeaderLabel.Text; }
		set { _rightHeaderLabel.Text = value; }
	}

	[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public object[] LeftItems {
		get { return _leftListBox.Items.Cast<object>().ToArray(); }
		set {
			_leftListBox.Items.Clear();
			_leftListBox.Items.AddRange(value);
		}
	}

	[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public object[] RightItems {
		get { return _rightListBox.Items.Cast<object>().ToArray(); }
		set {
			_rightListBox.Items.Clear();
			_rightListBox.Items.AddRange(value);
		}
	}

	#endregion

	#region Methods

	public void ClearLists() {
		LeftItems = RightItems = new object[0];
	}

	protected virtual void OnItemMoved(ListMergerSide from, ListMergerSide to, ListMergeMode action, object item) {
	}

	protected virtual void OnItemSelected(ListMergerSide side, object selectedItem) {
	}

	protected virtual void CopyToLeft() {
		var selectedItems = _rightListBox.SelectedItems.Cast<object>().ToArray();
		_leftListBox.Items.AddRange(selectedItems);
		RaiseItemsMovedEvent(ListMergerSide.Right, ListMergerSide.Left, ListMergeMode.CopyRightToLeft, selectedItems);
	}

	protected virtual void MoveToLeft() {
		var selectedItems = _rightListBox.SelectedItems.Cast<object>().ToArray();
		_leftListBox.Items.AddRange(selectedItems);
		_rightListBox.RemoveSelectedItems();
		RaiseItemsMovedEvent(ListMergerSide.Right, ListMergerSide.Left, ListMergeMode.Move, selectedItems);
	}

	protected virtual void RemoveFromLeft() {
		var selectedItems = _rightListBox.SelectedItems.Cast<object>().ToArray();
		_leftListBox.RemoveSelectedItems();
		RaiseItemsMovedEvent(ListMergerSide.Left, ListMergerSide.Right, ListMergeMode.CopyRightToLeft, selectedItems);
	}

	protected virtual void CopyToRight() {
		var selectedItems = _leftListBox.SelectedItems.Cast<object>().ToArray();
		_rightListBox.Items.AddRange(selectedItems);
		RaiseItemsMovedEvent(ListMergerSide.Left, ListMergerSide.Right, ListMergeMode.CopyLeftToRight, selectedItems);
	}

	protected virtual void MoveToRight() {
		object[] selectedItems = _leftListBox.SelectedItems.Cast<object>().ToArray();
		_rightListBox.Items.AddRange(selectedItems);
		_leftListBox.RemoveSelectedItems();
		RaiseItemsMovedEvent(ListMergerSide.Left, ListMergerSide.Right, ListMergeMode.Move, selectedItems);
	}

	protected virtual void RemoveFromRight() {
		var selectedItems = _rightListBox.SelectedItems.Cast<object>().ToArray();
		_rightListBox.RemoveSelectedItems();
		RaiseItemsMovedEvent(ListMergerSide.Right, ListMergerSide.Left, ListMergeMode.CopyLeftToRight, selectedItems);
	}

	#endregion

	#region Handlers

	private void _moveLeftButton_Click(object sender, EventArgs e) {
		try {
			switch (MergeMode) {
				case ListMergeMode.CopyLeftToRight:
					RemoveFromRight();
					break;
				case ListMergeMode.CopyRightToLeft:
					CopyToLeft();
					break;
				case ListMergeMode.Move:
				default:
					MoveToLeft();
					break;
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void _moveRightButton_Click(object sender, EventArgs e) {
		try {
			switch (MergeMode) {
				case ListMergeMode.CopyLeftToRight:
					CopyToRight();
					break;
				case ListMergeMode.CopyRightToLeft:
					RemoveFromLeft();
					break;
				case ListMergeMode.Move:
				default:
					MoveToRight();
					break;
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	//private void ListMerger_Resize(object sender, EventArgs e) {
	//    _leftListBox.Width = Width / 2 - 34 / 2;
	//    _leftListBox.Height = Height - 15;
	//    _leftHeaderLabel.Width = _leftListBox.Width;

	//    _rightListBox.Width = _leftListBox.Width;
	//    _rightListBox.Location = new Point(Width - _rightListBox.Width, _rightListBox.Location.Y);
	//    _rightListBox.Height = Height - 15;
	//    _rightHeaderLabel.Width = _rightListBox.Width;
	//    _rightHeaderLabel.Location = new Point(_rightListBox.Location.X, _rightHeaderLabel.Location.Y);


	//    _moveLeftButton.Location
	//        = new Point(
	//            Width / 2 - _moveLeftButton.Width / 2,
	//            Height / 2 - 6 / 2 - _moveLeftButton.Height
	//          );

	//    _moveRightButton.Location
	//        = new Point(
	//            Width / 2 - _moveRightButton.Width / 2,
	//            Height / 2 + 6 / 2
	//          );
	//}

	private void _rightListBox_SelectedIndexChanged(object sender, EventArgs e) {
		if (_rightListBox.SelectedIndex != -1) {
			_leftListBox.ClearSelected();
			RaiseItemSelectedEvent(
				ListMergerSide.Right,
				_rightListBox.Items[_rightListBox.SelectedIndex]
			);
		}
	}

	private void _leftListBox_SelectedIndexChanged(object sender, EventArgs e) {
		if (_leftListBox.SelectedIndex != -1) {
			_rightListBox.ClearSelected();
			RaiseItemSelectedEvent(
				ListMergerSide.Left,
				_leftListBox.Items[_leftListBox.SelectedIndex]
			);
		}
	}

	private void _leftListBox_KeyDown(object sender, KeyEventArgs e) {
		if (e.KeyCode == Keys.A && e.Control) {
			for (int i = 0; i < _leftListBox.Items.Count; i++) {
				_leftListBox.SetSelected(i, true);
			}
			e.SuppressKeyPress = true;
		}
	}

	private void _rightListBox_KeyDown(object sender, KeyEventArgs e) {
		if (e.KeyCode == Keys.A && e.Control) {
			for (int i = 0; i < _rightListBox.Items.Count; i++) {
				_rightListBox.SetSelected(i, true);
			}
			e.SuppressKeyPress = true;
		}
	}

	#endregion

	#region Event Triggers

	private void RaiseItemsMovedEvent(ListMergerSide from, ListMergerSide to, ListMergeMode action, object[] items) {
		OnItemMoved(from, to, action, items);
		if (ItemsMoved != null) {
			ItemsMoved(this, from, to, action, items);
		}

	}

	private void RaiseItemSelectedEvent(ListMergerSide side, object item) {
		OnItemSelected(side, item);
		if (ItemSelected != null) {
			ItemSelected(this, side, item);
		}
	}

	#endregion

}
