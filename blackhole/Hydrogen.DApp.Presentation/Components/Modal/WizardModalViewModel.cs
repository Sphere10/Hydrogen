// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.Components.Wizard;
using Hydrogen.DApp.Presentation.ViewModels;

namespace Hydrogen.DApp.Presentation.Components.Modal;

/// <summary>
/// Wizard modal view model
/// </summary>
public class WizardModalViewModel : ModalViewModel {
	/// <summary>
	/// Gets or sets the wizard being hosted in the modal.
	/// </summary>
	public IWizard Wizard { get; set; } = null!;

	/// <summary>
	/// Gets or sets the wizard host component instance.
	/// </summary>
	public WizardHost? WizardHost;

	/// <summary>
	/// Modal closed result. Passes request to the wizard instance to determine whether close OK.
	/// </summary>
	public override async Task<bool> RequestCloseAsync() {
		Result<bool> result = await Wizard.CancelAsync();

		if (result) {
			await base.RequestCloseAsync();
			return result;
		} else {


			WizardHost?.ViewModel!.ErrorMessages.Clear();
			WizardHost?.ViewModel!.ErrorMessages.AddRange(result.ErrorMessages);
			return result;
		}
	}
}
