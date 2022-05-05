//-----------------------------------------------------------------------
// <copyright file="EnumComboBox.cs" company="Sphere 10 Software">
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Hydrogen;

namespace Hydrogen.Windows.Forms {
	public class EnumComboBox : ComboBoxEx {
		private Type _enumType;
	    private object[] _ignoreVals;

		public EnumComboBox() {
            _ignoreVals = new object[0];
            AllowEmptyOption = false;
			EmptyOptionText = string.Empty;
			EnumType = typeof(NoOpEnum);
			base.DropDownStyle = ComboBoxStyle.DropDownList;
		}

		public bool AllowEmptyOption { get; set; }

		public string EmptyOptionText { get; set; }



		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Type EnumType {
			get {
				return _enumType;
			}
			set {
				if (value != null && !value.IsEnum)
					throw new ArgumentException("Type is not enum", "value");
				_enumType = value;
				if (!DesignMode)
					PopulateCombo();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Enum SelectedEnum {
			get {
				if (SelectedIndex == -1)
					return null;

				if (AllowEmptyOption && SelectedIndex == 0)
					return null;

				return (Enum) ((ComboItem)this.Items[this.SelectedIndex]).Value;
			}
			set {
				if (value == null && !AllowEmptyOption)
					throw new ArgumentNullException("NULL options are not allowed on this combo-box.", "value");

				if (value == null)
					SelectedIndex = 0;
				else
					this.TrySelectByText(value.GetDescription());
			}
		}

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object[] IgnoreEnums {
            get { return _ignoreVals; }
            set {
                _ignoreVals = value ?? new object[0];
                if (!DesignMode && _enumType != null)
                    PopulateCombo();
            }
        }


        private void PopulateCombo() {
			var comboItems = new List<ComboItem>();
			if (AllowEmptyOption) {
				comboItems.Add(new ComboItem { Display = EmptyOptionText ?? string.Empty, Value = null } );
			}
			if (_enumType != null) {
				foreach (Enum enumValue in Enum.GetValues(_enumType)) {
                    if (_ignoreVals.Contains(enumValue))
                        continue;
					comboItems.Add(
						new ComboItem {
							Display = Tools.Enums.GetDescription(enumValue),
							Value = enumValue
						}
					);
				}
			}
			base.Items.Clear();
			base.DisplayMember = "Display";
			base.ValueMember = "Value";
			base.Items.AddRange(comboItems.Cast<object>().ToArray());

			if (Items.Count > 0)
				base.SelectedIndex = 0;
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object DataSource { get => base.DataSource; set => base.DataSource = value; }


		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ObjectCollection Items => base.Items;
		

		private class ComboItem {
			public string Display { get; set; }
			public object Value { get; set; }
		}

		public enum NoOpEnum {
		}

	}


	


}
