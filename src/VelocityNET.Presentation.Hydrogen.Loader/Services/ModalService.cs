using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.Components.Modal;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;
using VelocityNET.Presentation.Hydrogen.Services;

namespace VelocityNET.Presentation.Hydrogen.Loader.Services
{

    public class ModalService : IModalService
    {
        private ModalHost? ModalInstance { get; set; }

        /// <summary>
        /// Initialize the modal service passing a reference to the modal host component. Must be completed
        /// before this object may be used.
        /// </summary>
        /// <param name="component"> modal host</param>
        public void Initialize(ModalHost component)
        {
            ModalInstance = component ?? throw new ArgumentNullException(nameof(component));
        }

        /// <summary>
        /// Show the modal component of type,
        /// <typeparam name="T"> modal component to show. must implement modal component</typeparam>
        /// </summary>
        /// <param name="parameters"> parameters to supply to the modal component.</param>
        /// <returns> modal result</returns>
        public async Task<ModalResult> ShowAsync<T>(Dictionary<string, object>? parameters = null)
            where T : ModalComponentBase
        {
            if (ModalInstance is null)
            {
                throw new InvalidOperationException("Modal service is not initialized, no modal component");
            }
            
            return await ModalInstance.ShowAsync<T>(parameters is null
                ? ParameterView.Empty
                : ParameterView.FromDictionary(parameters));
        }

        /// <summary>
        /// Show a wizard modal, with the supplied wizard model.
        /// </summary>
        /// <param name="wizard"> wizard</param>
        /// <returns> modal result.</returns>
        public async Task<ModalResult> ShowWizardAsync(IWizard wizard, Dictionary<string, object>? parameters = null)
        {
            if (ModalInstance is null)
            {
                throw new InvalidOperationException("Modal service is not initialized, no modal component");
            }

            if (parameters is null)
            {
                parameters = new Dictionary<string, object>()
                {
                    {nameof(WizardModal.Wizard), wizard}
                };
            }
            else
            {
                parameters.Add(nameof(WizardModal.Wizard), wizard);
            }

            return await ModalInstance.ShowAsync<WizardModal>(ParameterView.FromDictionary(parameters));
        }
    }
}