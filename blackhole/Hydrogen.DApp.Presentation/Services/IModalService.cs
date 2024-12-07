// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.Components.Modal;
using Hydrogen.DApp.Presentation.Components.Wizard;

namespace Hydrogen.DApp.Presentation.Services;

public interface IModalService {
	/// <summary>
	/// Initialize the modal service passing a reference to the modal component.
	/// </summary>
	/// <param name="component"></param>
	void Initialize(ModalHost component);

	/// <summary>
	/// Show the modal component of type,
	/// <typeparam name="T"> modal component to show. must implement modal component</typeparam>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="parameters"> parameters to supply to the modal component.</param>
	/// <returns></returns>
	Task<ModalResult> ShowAsync<T>(Dictionary<string, object>? parameters = null)
		where T : ModalComponentBase;

	/// <summary>
	/// Show a wizard modal, with the supplied wizard model.
	/// </summary>
	/// <param name="wizard"> wizard</param>
	/// <param name="parameters"></param>
	/// <returns> modal result.</returns>
	Task<ModalResult> ShowWizardAsync(IWizard wizard, Dictionary<string, object>? parameters = null);
}
