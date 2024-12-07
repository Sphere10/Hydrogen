using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Hydrogen.DApp.Presentation.Components.Wizard;

/// <summary>
/// Wizard step component base. 
/// </summary>
/// <typeparam name="TModel"> model type</typeparam>
/// <typeparam name="TViewModel"> view model type</typeparam>
public abstract partial class WizardStep<TModel, TViewModel>
	where TViewModel : WizardStepViewModelBase<TModel> {
	/// <summary>
	/// Gets or sets the step view model
	/// </summary>
	[Inject]
	// ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
	public TViewModel ViewModel { get; set; } = null!;

	/// <summary>
	/// Gets or sets the wizard instance
	/// </summary>
	[Parameter]
	public IWizard Wizard {
		get => ViewModel!.Wizard;
		set => ViewModel!.Wizard = (IWizard<TModel>)value;
	}

	/// <inheritdoc />
	public override Task<Result> OnNextAsync() => ViewModel!.OnNextAsync();

	/// <inheritdoc />
	public override Task<Result> OnPreviousAsync() => ViewModel!.OnPreviousAsync();

	/// <inheritdoc />
	protected override void OnParametersSet() {
		base.OnParametersSet();
		if (Wizard is null) {
			throw new InvalidOperationException("Wizard step requires wizard parameter be set.");
		}
	}
}


/// <summary>
/// Non generic wizard step component base.
/// </summary>
public abstract class WizardStepBase : ComponentBase {
	/// <summary>
	/// Gets or sets the title of the wizard step.
	/// </summary>
	public abstract string Title { get; }

	/// <summary>
	/// Gets the next button text for this step.
	/// </summary>
	public virtual string NextButtonText { get; } = "Next";

	/// <summary>
	/// Gets the back button text for this step.
	/// </summary>
	public virtual string BackButtonText { get; } = "Back";

	/// <summary>
	/// Gets the cancel button text for this step.
	/// </summary>
	public virtual string CancelButtonText { get; } = "Cancel";

	/// <summary>
	/// Gets the finish button text.
	/// </summary>
	public virtual string FinishButtonText { get; } = "Finish";

	/// <summary>
	/// Gets a value indicating whether the wizard / step may be cancelled.
	/// </summary>
	public virtual bool IsCancellable { get; } = true;

	/// <summary>
	/// Called when the wizard requests the next step. Returning true will allow
	/// the wizard to progress.
	/// </summary>
	/// <returns> whether or not the step is finished and to move next</returns>
	public abstract Task<Result> OnNextAsync();

	/// <summary>
	/// Called when the wizard requests the prev step. Returning true will allow
	/// the wizard to progress.
	/// </summary>
	/// <returns> whether or not the step is finished and to move prev</returns>
	public abstract Task<Result> OnPreviousAsync();
}
