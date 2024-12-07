using System;
using Microsoft.AspNetCore.Components;

namespace Hydrogen.DApp.Presentation.Components.Wizard;

/// <summary>
/// Wizard component.
/// </summary>
// HS: almost all of this should be merged into WizardViewModel<TModel>
public partial class WizardHost {
	/// <summary>
	/// Call back, invoked when wizard is finished. cascaded from a parent component is used to signal
	/// the completion of the wizard.
	/// </summary>
	[CascadingParameter(Name = "OnFinished")]
	public EventCallback OnFinished {
		get => ViewModel!.OnFinished;
		set => ViewModel!.OnFinished = value;
	}

	/// <summary>
	/// Call back, invoked when wizard is cancelled. cascaded from a parent component is used to signal
	/// the cancellation of the wizard.
	/// </summary>
	[CascadingParameter(Name = "OnCancelled")]
	public EventCallback OnCancelled {
		get => ViewModel!.OnCancelled;
		set => ViewModel!.OnCancelled = value;
	}

	/// <summary>
	/// Call back, invoked when step changes - used to notify parent component.
	/// </summary>
	[CascadingParameter(Name = "OnStepChange")]
	public EventCallback OnStepChange {
		get => ViewModel!.OnStepChange;
		set => ViewModel!.OnStepChange = value;
	}

	/// <summary>
	/// Gets or sets the wizard model instance.
	/// </summary>
	[CascadingParameter]
	public IWizard Wizard {
		get => ViewModel!.Wizard;
		set => ViewModel!.Wizard = value;
	}

	/// <summary>
	/// Gets or sets the css style for the next button
	/// </summary>
	[CascadingParameter]
	public string? NextButtonClass { get; set; }

	/// <summary>
	/// Gets or sets the css style for the back button
	/// </summary>
	[CascadingParameter]
	public string? BackButtonClass { get; set; }

	/// <summary>
	/// Gets or sets the css style for the cancel button
	/// </summary>
	[CascadingParameter]
	public string? CancelButtonClass { get; set; }

	/// <summary>
	/// Gets or sets the css style for the finish button
	/// </summary>
	[CascadingParameter]
	public string? FinishButtonClass { get; set; }

	/// <inheritdoc />
	protected override void OnParametersSet() {
		base.OnParametersSet();

		if (Wizard is null) {
			throw new InvalidOperationException("Wizard parameter is required.");
		}
	}
}
