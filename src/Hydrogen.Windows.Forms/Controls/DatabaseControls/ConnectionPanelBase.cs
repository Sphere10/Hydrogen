//-----------------------------------------------------------------------
// <copyright file="ConnectionPanelBase.cs" company="Sphere 10 Software">
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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen;
using Hydrogen.Data;

namespace Hydrogen.Windows.Forms {
	public partial class ConnectionPanelBase : UserControlEx, IDatabaseConnectionProvider {
		public ConnectionPanelBase() {
			InitializeComponent();
		}

		public IDAC GetDAC() {
			var dac = GetDACInternal();
			return dac;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public virtual string ConnectionString {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual string DatabaseName {
            get { throw new NotImplementedException(); }
        }

		protected virtual IDAC GetDACInternal() {
			throw new NotImplementedException();
		}



        public virtual async Task<Result> TestConnection() {
            var result = Result.Default;
            var dac = GetDAC();
            try {
                await Task.Run(() => {
                    using (var scope = dac.BeginScope(openConnection: true)) {

                    }
                });
            } catch (Exception error) {
                result.AddError(error.ToDisplayString());
            }
            return result;
        }

	}
}
