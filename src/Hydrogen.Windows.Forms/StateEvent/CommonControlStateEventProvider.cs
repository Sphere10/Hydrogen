//-----------------------------------------------------------------------
// <copyright file="CommonControlStateEventProvider.cs" company="Sphere 10 Software">
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
using System.Text;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Hydrogen.Application;
using Hydrogen;

namespace Hydrogen.Windows.Forms {

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
                TypeSwitch.Default(() => {
                    throw new SoftwareException($"Control '{control.GetType().Name}' is not supported");
                })
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
                TypeSwitch.Default(() => {
                    throw new SoftwareException($"Control '{control.GetType().Name}' is not supported");
                })
            );
        }

        private EventHandler ToCompatibleHandler(EventHandlerEx eventHandler)
            => (o, a) => eventHandler();
    }
}
