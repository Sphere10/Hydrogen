using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sphere10.Hydrogen.Presentation2.Logic.Wizard;
using Sphere10.Hydrogen.Presentation2.UI.Dialogs;
using Sphere10.Hydrogen.Presentation2.UI.Wizard;

namespace Sphere10.Hydrogen.Presentation2.Logic.Modal
{

    /// <summary>
    /// View service - provides common services to views.
    /// </summary>
    public static class ViewService
    {
        /// <summary>
        /// Show a wizard modal, with the supplied wizard model.
        /// </summary>
        /// <param name="wizard"> wizard</param>
        /// <param name="options"></param>
        /// <returns> modal result.</returns>
        public static async Task<ModalResult> WizardDialogAsync(IWizard wizard, ModalOptions options = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {nameof(WizardHost.Wizard), wizard}
            };

            return await ModalService.ShowAsync<WizardModal>(ParameterView.FromDictionary(parameters), options);
        }

        /// <summary>
        /// Show confirm dialog, and await user interaction result
        /// </summary>
        /// <param name="title"> dialog title</param>
        /// <param name="message"></param>
        /// <param name="confirmText"></param>
        /// <returns>a value indicating whether the user confirmed or cancelled/closed dialog.</returns>
        public static async Task<bool>
            ConfirmDialogAsync(string title, string message, string confirmText = "OK")
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {nameof(ConfirmDialog.Title), title},
                {nameof(ConfirmDialog.Message), message},
                {nameof(ConfirmDialog.ConfirmMessageText), confirmText}
            };

            var result = await ModalService.ShowAsync<ConfirmDialog>(ParameterView.FromDictionary(parameters),
                new ModalOptions {Size = ModalSize.Small});
            
            return result.ResultType == ModalResultType.Ok;
        }

        /// <summary>
        /// Show info dialog
        /// </summary>
        /// <param name="title"> title of dialog</param>
        /// <param name="message"> message </param>
        /// <param name="confirmText"> optional confirm button text</param>
        /// <returns>a task that is complete once the user has dismissed the modal</returns>
        public static async Task
            InfoDialogAsync(string title, string message, string confirmText = "OK")
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {nameof(InfoDialog.Title), title},
                {nameof(InfoDialog.Message), message},
                {nameof(InfoDialog.ConfirmMessageText), confirmText}
            };

           await ModalService.ShowAsync<InfoDialog>(ParameterView.FromDictionary(parameters));
        }
    }
}