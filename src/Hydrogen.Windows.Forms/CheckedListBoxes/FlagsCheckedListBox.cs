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
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public class FlagsCheckedListBox : CheckedListBoxEx {
	private Type _enumType;
	private string[] _enumNames;
	private Enum[] _enumValues;
	private Int64[] _enumNumericValues;
	private ILookup<Enum, int> _enumToIndexLookup;
	private ILookup<Enum, Enum> _parentEnumLookup;
	private ILookup<Enum, Enum> _childEnumLookup;
	private bool _suspendEvents;

	public FlagsCheckedListBox() {
		_suspendEvents = false;
		CheckOnClick = true;
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Type EnumType {
		get { return _enumType; }
		set {
			if (value == null) {
				base.Items.Clear();
				_enumType = null;
				_enumValues = null;
				_enumNames = null;
				_enumNumericValues = null;
				_enumToIndexLookup = null;
				_parentEnumLookup = null;
				_childEnumLookup = null;
				return;
			}
			if (value != null && !value.IsEnum)
				throw new ArgumentException("Type is not enum", "value");

			if (!Attribute.IsDefined(value, typeof(FlagsAttribute)))
				throw new ArgumentException("Type dos not have 'Flags' attribute", "value");

			_enumType = value;
			_enumValues = Enum.GetValues(_enumType).Cast<Enum>().ToArray();
			_enumNames = _enumValues.Select(ev => ev.GetDescription()).ToArray();
			_enumNumericValues = (from e in _enumValues select Convert.ToInt64(e)).ToArray();
			_enumToIndexLookup = _enumValues.WithDescriptions().ToLookup(e => e.Item, e => e.Index);
			var enumComponentPairs = (
				from e in _enumValues
				from c in GetComponentFlags(e)
				select new {
					Parent = e,
					Child = c
				}
			).ToArray();
			_parentEnumLookup = enumComponentPairs.ToLookup(pair => pair.Child, pair => pair.Parent);
			_childEnumLookup = enumComponentPairs.ToLookup(pair => pair.Parent, pair => pair.Child);
			if (!DesignMode)
				PopulateCheckedListBox();
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Enum SelectedEnum {
		get {
			if (_enumType == null || !HasSelectedValue)
				return null;

			Debug.Assert(base.Items.Count == _enumValues.Count(), "base.Items.Count == enumValues.Count()");
			var checkedArray = Enumerable.Range(0, base.Items.Count).Select(i => base.GetItemChecked(i)).ToArray();

			// Compile the enum
			Int64 value = 0;
			for (var i = 0; i < checkedArray.Length; i++) {
				if (checkedArray[i]) {
					value |= _enumNumericValues[i];
				}
			}

			// Parse it back to its enum type
			return (Enum)Enum.Parse(_enumType, value.ToString());
		}
		set {

			// No enum type specified, ignore set value 
			if (_enumType == null)
				return;

			// Setting to null, so clear all checkboxes
			if (value == null) {
				for (int i = 0; i < base.Items.Count; i++)
					SetItemChecked(i, false);
				return;
			}

			// If supplied enum doesn't match declaring type, its an error
			if (value.GetType() != _enumType) {
				throw new ArgumentOutOfRangeException(string.Format("value is not of type '{0}'", _enumType.Name), "value");
			}

			// Check all the relevant boxes using enum value
			CheckBoxesUsingEnum(value);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool HasSelectedValue {
		get { return CheckedItems.Count > 0; }
	}

	protected override void OnItemCheck(ItemCheckEventArgs ice) {
		base.OnItemCheck(ice);
		if (_suspendEvents)
			return;
		try {
			_suspendEvents = true;
			var enumChanged = _enumValues[ice.Index];
			switch (ice.NewValue) {
				case CheckState.Checked:
					// for all child enums that are off, turn them on
					foreach (var childEnum in _childEnumLookup[enumChanged]) {
						var comboIndexes = _enumToIndexLookup[childEnum];
						foreach (var comboIndex in comboIndexes) {
							if (!GetItemChecked(comboIndex))
								SetItemChecked(comboIndex, true);
						}
					}

					// for all parent enums that are off, if all their children are checked (and the parent is the sum of its child) then check parent on
					foreach (var parentEnum in _parentEnumLookup[enumChanged]) {
						var allChildrenChecked =
							from childOfParent in _childEnumLookup[parentEnum]
							let comboIndexes = _enumToIndexLookup[childOfParent]
							from comboIndex in comboIndexes
							select comboIndex == ice.Index || base.GetItemChecked(comboIndex);

						Int64 allChildrenValue =
							_childEnumLookup[parentEnum]
								.Aggregate(
									(Int64)0,
									(currVal, e) =>
										currVal |
										_enumToIndexLookup[e]
											.Aggregate(
												(Int64)0,
												(s, i) => s | _enumNumericValues[i]
											)
								);
						if (allChildrenChecked.All(@checked => @checked)) {
							_enumToIndexLookup[parentEnum].ForEach(comboIndex => {
									if (_enumNumericValues[comboIndex] == allChildrenValue)
										SetItemChecked(comboIndex, true);
								}
							);
						}
					}
					break;
				case CheckState.Unchecked:
					// if parent is the sum of its children, for all child enums that are on, turn them off
					Int64 allChildrenValue2 =
						_childEnumLookup[enumChanged]
							.Aggregate(
								(Int64)0,
								(currVal, e) =>
									currVal |
									_enumToIndexLookup[e]
										.Aggregate(
											(Int64)0,
											(s, i) => s | _enumNumericValues[i]
										)
							);
					if (_enumNumericValues[ice.Index] == allChildrenValue2) {
						foreach (var childEnum in _childEnumLookup[enumChanged]) {
							var comboIndexes = _enumToIndexLookup[childEnum];
							foreach (var comboIndex in comboIndexes) {
								if (GetItemChecked(comboIndex))
									SetItemChecked(comboIndex, false);
							}
						}
					}

					// for all parent enums that are on, turn them off
					foreach (var @enum in _parentEnumLookup[enumChanged]) {
						var comboIndexes = _enumToIndexLookup[@enum];
						foreach (var comboIndex in comboIndexes) {
							if (GetItemChecked(comboIndex))
								SetItemChecked(comboIndex, false);
						}
					}

					break;

			}
		} finally {
			_suspendEvents = false;
		}
	}

	protected override bool CanRaiseEvents {
		get { return base.CanRaiseEvents && !_suspendEvents; }
	}

	private void CheckBoxesUsingEnum(Enum value) {
		try {
			BeginUpdate();
			Debug.Assert(base.Items.Count == _enumValues.Count(), "base.Items.Count == enumValues.Count()");
			var lValue = Convert.ToInt64(value);
			for (var i = 0; i < base.Items.Count; i++) {
				try {
					base.BeginUpdate();
					_suspendEvents = true;
					base.SetItemChecked(i, (lValue == 0 && _enumNumericValues[i] == 0) || ((lValue & _enumNumericValues[i]) == _enumNumericValues[i]));
				} finally {
					_suspendEvents = false;
					base.EndUpdate();
				}
			}
		} finally {
			EndUpdate();
		}
	}

	private void PopulateCheckedListBox() {
		try {
			base.BeginUpdate();
			_suspendEvents = true;
			base.Items.Clear();

			if (_enumType == null)
				return;

			var items = new List<CheckedBoxItem>();
			foreach (var enumName in _enumNames) {
				var item = new CheckedBoxItem {
					Display = enumName,
					Value = 0
				};
				items.Add(item);
			}
			base.DisplayMember = "Display";
			base.ValueMember = "Value";
			base.Items.AddRange(items.Cast<object>().ToArray());

			SelectedEnum = null;
		} finally {
			_suspendEvents = false;
			base.EndUpdate();
		}
	}

	private IEnumerable<Enum> GetComponentFlags(Enum value) {
		var lValue = Convert.ToInt64(value);
		return
			from flag in _enumValues
			let lFlag = Convert.ToInt64(flag)
			where lValue != 0 && lFlag != 0 && lValue != lFlag && (lValue & lFlag) == lFlag
			select flag;
	}


	private class CheckedBoxItem {
		public string Display { get; set; }
		public object Value { get; set; }
	}
}
