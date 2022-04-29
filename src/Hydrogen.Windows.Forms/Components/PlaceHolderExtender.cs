//-----------------------------------------------------------------------
// <copyright file="PlaceHolderExtender.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sphere10.Framework.Windows;

namespace Sphere10.Framework.Windows.Forms {
    [ToolboxItemFilter("System.Windows.Forms")]
    [ProvideProperty("PlaceHolderText", typeof(Control))]
    [Category("Behavior")]
    public class PlaceHolderTextExtender : Component, IExtenderProvider {
        private readonly Dictionary<Control, string> _placeHolderTexts = new Dictionary<Control, string>();
        private Container _components = null;

        public PlaceHolderTextExtender(IContainer container)
            : this() {
            container.Add(this);
        }

        public PlaceHolderTextExtender()
            : base() {
            this._components = new Container();
        }

        public bool CanExtend(object extendee) {
            return extendee is TextBoxBase || extendee is ComboBox;
        }

        public string GetPlaceHolderText(Control control) {
            if (_placeHolderTexts.ContainsKey(control)) {
                return _placeHolderTexts[control];
            }
            return string.Empty;
        }

        public void SetPlaceHolderText(Control control, string cueText) {
            var handle = control.Handle;
            if (!_placeHolderTexts.ContainsKey(control)) {
                _placeHolderTexts.Add(control, string.Empty);
            }
            _placeHolderTexts[control] = cueText;
            if (control is ComboBox) {
                handle = WinAPI.USER32.GetWindow(control.Handle, WinAPI.USER32.GetWindow_Cmd.GW_CHILD);
            }
            WinAPI.USER32.SendMessage(handle, WinAPI.WindowMessageFlags.EM_SETCUEBANNER, IntPtr.Zero, cueText);
        }
    }
}
