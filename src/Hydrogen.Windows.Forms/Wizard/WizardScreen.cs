//-----------------------------------------------------------------------
// <copyright file="WizardScreen.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Hydrogen.Windows.Forms;

public class WizardScreen<T> : UserControlEx, IWizardScreen<T> {
	public IWizard<T> Wizard { get; internal set; }

	public T Model => Wizard.Model;

	public virtual async Task Initialize() {
	}

	public virtual async Task OnPresent() {
		CopyModelToUI();
	}

	public virtual async Task OnPrevious() {
	}

	public virtual async Task OnNext() {
	}

	public virtual async Task<Result> Validate() {
		return Result.Default;
	}

}
