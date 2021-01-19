using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{
    public class NewWidgetWizardStepViewModel : WizardStepComponentViewModelBase
    {
        /// <inheritdoc />
        public override Task<bool> OnNextAsync()
        {
            return Task.FromResult(true);
        }


        /// <inheritdoc />
        public override Task<bool> OnPreviousAsync()
        {
            return Task.FromResult(true);
        }
    }
}