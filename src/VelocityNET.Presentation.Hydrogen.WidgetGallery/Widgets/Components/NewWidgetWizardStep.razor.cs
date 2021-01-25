using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Components
{
    public partial class NewWidgetWizardStep
    {
        /// <summary>
        /// Gets a value indicating whether the wizard / step may be cancelled.
        /// </summary>
        public override bool IsCancellable { get; } = false;

        /// <summary>
        /// Gets or sets the title of the wizard step.
        /// </summary>
        public override string Title { get; } = "New Widget";
        
        /// <inheritdoc />
        public override async Task<bool> OnNextAsync()
        {
            bool success = await base.OnNextAsync();

            if (success)
            {
                if (Model!.AreDimensionsKnown)
                {
                    Wizard.UpdateSteps(StepUpdateType.Inject, typeof(WidgetDimensionsStep));
                }
            }

            return success;
        }
    }
}