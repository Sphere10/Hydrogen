// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Hydrogen.Data;

namespace Hydrogen.Windows.Forms;

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
