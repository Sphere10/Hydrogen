//-----------------------------------------------------------------------
// <copyright file="WhoAreYouControl.cs" company="Sphere 10 Software">
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Hydrogen.Application;

namespace Hydrogen.Windows.Forms {

    public partial class WhoAreYouControl : ApplicationControl {
        public WhoAreYouControl() {
            InitializeComponent();
        }

        public UserType UserType {
            get {
                UserType retval = UserType.HomeUser;
                if (_homeUserRadioButton.Checked) {
                    retval = UserType.HomeUser;
                } else if (_smallBusinessRadioButton.Checked) {
                    retval = UserType.SmallBusiness;
                } else if (_mediumBusinessRadioButton.Checked) {
                    retval = UserType.MediumBusiness;
                } else if (_corporationRadioButton.Checked) {
                    retval = UserType.Corporation;
                }
                return retval;
            }
            set {
                switch (value) {
                    case UserType.HomeUser:
                        _homeUserRadioButton.Checked = true;
                        break;
                    case UserType.SmallBusiness:
                        _smallBusinessRadioButton.Checked = true;
                        break;
                    case UserType.MediumBusiness:
                        _mediumBusinessRadioButton.Checked = true;
                        break;
                    case UserType.Corporation:
                        _corporationRadioButton.Checked = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
