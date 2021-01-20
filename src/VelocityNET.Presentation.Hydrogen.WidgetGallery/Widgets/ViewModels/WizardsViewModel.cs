using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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

        private IWizardBuilder Builder { get; }

        /// <summary>
        /// Gets list of widgets 
        /// </summary>
        public List<NewWidgetModel> Widgets { get; } = new();

        /// <summary>
        /// Wizards view model
        /// </summary>
        /// <param name="modalService"></param>
        /// <param name="builder"></param>
        public WizardsViewModel(IModalService modalService, IWizardBuilder builder)
        {
            ModalService = modalService;
            Builder = builder;
        }

        /// <summary>
        /// Display the new widget modal and add the result to the collection of widgets.
        /// </summary>
        /// <returns></returns>
        public async Task ShowNewWidgetModalAsync()
        {
            RenderFragment wizard = Builder.NewWizard<Wizard>()
                .WithModel(new NewWidgetModel())
                .AddStep<NewWidgetWizardStep>()
                .AddStep<NewWidgetSummaryStep>()
                .Build();

            var result = await ModalService.ShowAsync<WizardModal>(new Dictionary<string, object>
            {
                {nameof(WizardModal.Wizard), wizard},
            });


            if (result.ResultType is ModalResultType.Ok)
            {
                NewWidgetModel model = result.GetData<NewWidgetModel>();
                Widgets.Add(model);
            }
        }
    }

}