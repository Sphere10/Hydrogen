// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Microsoft.AspNetCore.Components;
using Hydrogen.DApp.Presentation.Components.Wizard;

namespace Hydrogen.DApp.Presentation.Components.Modal;

/// <summary>
/// Wizard modal - show a wizard component inside a modal dialog.
/// </summary>
public partial class WizardModal {
	/// <summary>
	/// Gets or sets the wizard render fragment
	/// </summary>
	[Parameter]
	public IWizard Wizard {
		get => ViewModel!.Wizard;
		set => ViewModel!.Wizard = value;
	}

	/// <summary>
	/// Gets or sets the css style for the next button
	/// </summary>
	[Parameter]
	public string? NextButtonClass { get; set; }

	/// <summary>
	/// Gets or sets the css style for the back button
	/// </summary>
	[Parameter]
	public string? BackButtonClass { get; set; }

	/// <summary>
	/// Gets or sets the css style for the cancel button
	/// </summary>
	[Parameter]
	public string? CancelButtonClass { get; set; }

	/// <summary>
	/// Gets or sets the css style for the finish button
	/// </summary>
	[Parameter]
	public string? FinishButtonClass { get; set; }

	/// <summary>
	/// Gets or sets the call back the wizard will use when finished to signal the completion of the
	/// modal interaction.
	/// </summary>
	private EventCallback OnFinished { get; set; }

	/// <summary>
	/// Gets or sets the on cancelled call back the wizard will use when cancellation is requested.
	/// </summary>
	private EventCallback OnCancelled { get; set; }

	/// <summary>
	/// Gets or sets the event callback passed to child components to notify the wizard modal
	/// of step change.
	/// </summary>
	private EventCallback OnStepChange { get; set; }

	/// <inheritdoc />
	protected override void OnParametersSet() {
		base.OnParametersSet();

		if (Wizard is null) {
			throw new InvalidOperationException("Wizard parameter is required");
		}

		OnFinished = EventCallback.Factory.Create(ViewModel!, ViewModel!.Ok);
		OnCancelled = EventCallback.Factory.Create(ViewModel!, ViewModel!.Cancel);
		OnStepChange = EventCallback.Factory.Create(this, StateHasChanged);
	}
}
