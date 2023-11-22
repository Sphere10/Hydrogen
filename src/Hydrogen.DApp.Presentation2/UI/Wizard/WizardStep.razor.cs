using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Hydrogen.DApp.Presentation2.Logic.Wizard;

namespace Hydrogen.DApp.Presentation2.UI.Wizard {

	/// <summary>
	/// Wizard step component base. 
	/// </summary>
	/// <typeparam name="TModel"> model type</typeparam>
	public abstract partial class WizardStep<TModel> {
		/// <summary>
		/// Gets or sets the wizard instance
		/// </summary>
		[Parameter]
		public IWizard<TModel> Wizard { get; set; }

		/// <summary>
		/// Gets the model.
		/// </summary>
		public TModel Model => Wizard.Model;
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
		/// Gets a value indicating whether the wizard / step may be cancelled.
		/// </summary>
		public bool IsCancellable { get; protected set; } = true;

		/// <summary>
		/// Called when the wizard requests the next step. Returning true will allow
		/// the wizard to progress.
		/// </summary>
		/// <returns> whether or not the step is finished and to move next</returns>
		public virtual Task<Result> OnNextAsync() => Task.FromResult(Result.Success);

		/// <summary>
		/// Called when the wizard requests the prev step. Returning true will allow
		/// the wizard to progress.
		/// </summary>
		/// <returns> whether or not the step is finished and to move prev</returns>
		public virtual Task<Result> OnPreviousAsync() => Task.FromResult(Result.Success);
	}
}
