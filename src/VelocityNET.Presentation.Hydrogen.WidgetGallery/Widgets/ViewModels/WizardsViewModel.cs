using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components;
using VelocityNET.Presentation.Hydrogen.Components.Modal;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Components;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Models;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{

    public class WizardsViewModel : ComponentViewModelBase
    {
        private IModalService ModalService { get; }

        public List<NewWidgetModel> Widgets { get; } = new();

        public WizardsViewModel(IModalService modalService)
        {
            ModalService = modalService;
        }

        /// <summary>
        /// Display the new widget modal and add the result to the collection of widgets.
        /// </summary>
        /// <returns></returns>
        public async Task ShowNewWidgetModalAsync()
        {
            var result = await ModalService.ShowAsync<WizardModal<Wizard<NewWidgetModel>>>(new Dictionary<string, object>
            {
                {nameof(Wizard<NewWidgetModel>.Model), new NewWidgetModel()},
                {
                    nameof(Wizard<NewWidgetModel>.Steps), new List<Type>
                    {
                        typeof(NewWidgetWizardStep),
                        typeof(NewWidgetSummaryStep),
                    }
                },
                {nameof(WizardModal<Wizard<NewWidgetModel>>.Title), "New Widget"}
            });

            if (result.ResultType is ModalResultType.Ok)
            {
                NewWidgetModel model = result.GetData<NewWidgetModel>();
                Widgets.Add(model);
            }
        }
    }

}