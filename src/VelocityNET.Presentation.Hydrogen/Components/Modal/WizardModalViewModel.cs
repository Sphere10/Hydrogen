using System.Threading.Tasks;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Components.Modal
{

    /// <summary>
    /// Wizard modal view model
    /// </summary>
    public class WizardModalViewModel : ModalViewModel
    {
        /// <summary>
        /// Gets or sets the wizard being hosted in the modal.
        /// </summary>
        public IWizard Wizard { get; set; } = null!;

        /// <summary>
        /// Modal closed result. Passes request to the wizard instance to determine whether close OK.
        /// </summary>
        public override async Task<bool> RequestCloseAsync()
        {
            Result<bool> result = await Wizard.CancelAsync();

            if (result)
            {
                await base.RequestCloseAsync();
                return result;
            }
            else
            {
                return result;
            }
        }
    }

}