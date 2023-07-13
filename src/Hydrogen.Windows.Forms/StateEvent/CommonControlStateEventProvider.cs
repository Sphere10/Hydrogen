// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public class CommonControlStateEventProvider : ControlStateEventProviderBase<Control> {

	protected override void RegisterStateChangedListener(Control control, EventHandlerEx eventHandler) {
		var compatibleHandler = ToCompatibleHandler(eventHandler);
		TypeSwitch.For(
			control,
			TypeSwitch.Case<NumericUpDown>(c => c.ValueChanged += compatibleHandler),
			TypeSwitch.Case<DecimalBox>(c => c.ValueChanged += compatibleHandler),
			TypeSwitch.Case<IntBox>(c => c.ValueChanged += compatibleHandler),
			TypeSwitch.Case<MoneyBox>(c => c.ValueChanged += compatibleHandler),
			TypeSwitch.Case<TextBox>(c => c.TextChanged += compatibleHandler),
			TypeSwitch.Case<ComboBox>(c => c.SelectedIndexChanged += compatibleHandler),
			TypeSwitch.Case<RadioButton>(c => c.CheckedChanged += compatibleHandler),
			TypeSwitch.Case<CheckBox>(c => c.CheckedChanged += compatibleHandler),
			TypeSwitch.Case<CheckedListBox>(c => c.SelectedIndexChanged += compatibleHandler),
			TypeSwitch.Case<DateTimePicker>(c => c.TextChanged += compatibleHandler),
			TypeSwitch.Case<ListBox>(c => c.SelectedIndexChanged += compatibleHandler),
			TypeSwitch.Default(() => { throw new SoftwareException($"Control '{control.GetType().Name}' is not supported"); })
		);
	}

	protected override void DeregisterStateChangedListener(Control control, EventHandlerEx eventHandler) {
		var compatibleHandler = ToCompatibleHandler(eventHandler);
		TypeSwitch.For(
			control,
			TypeSwitch.Case<NumericUpDown>(c => c.ValueChanged -= compatibleHandler),
			TypeSwitch.Case<DecimalBox>(c => c.ValueChanged -= compatibleHandler),
			TypeSwitch.Case<IntBox>(c => c.ValueChanged -= compatibleHandler),
			TypeSwitch.Case<MoneyBox>(c => c.ValueChanged -= compatibleHandler),
			TypeSwitch.Case<IntBox>(c => c.ValueChanged -= compatibleHandler),
			TypeSwitch.Case<TextBox>(c => c.TextChanged -= compatibleHandler),
			TypeSwitch.Case<ComboBox>(c => c.SelectedIndexChanged -= compatibleHandler),
			TypeSwitch.Case<RadioButton>(c => c.CheckedChanged -= compatibleHandler),
			TypeSwitch.Case<CheckBox>(c => c.CheckedChanged -= compatibleHandler),
			TypeSwitch.Case<CheckedListBox>(c => c.SelectedIndexChanged -= compatibleHandler),
			TypeSwitch.Case<DateTimePicker>(c => c.TextChanged -= compatibleHandler),
			TypeSwitch.Case<ListBox>(c => c.SelectedIndexChanged -= compatibleHandler),
			TypeSwitch.Default(() => { throw new SoftwareException($"Control '{control.GetType().Name}' is not supported"); })
		);
	}

	private EventHandler ToCompatibleHandler(EventHandlerEx eventHandler)
		=> (o, a) => eventHandler();
}
