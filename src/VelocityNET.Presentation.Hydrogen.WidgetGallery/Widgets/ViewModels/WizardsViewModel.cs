using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sphere10.Framework;
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

        private IWizardBuilder<NewWidgetModel> Builder { get; }

        /// <summary>
        /// Gets list of widgets 
        /// </summary>
        public List<NewWidgetModel> Widgets { get; } = new();

        /// <summary>
        /// Wizards view model
        /// </summary>
        /// <param name="modalService"></param>
        /// <param name="builder"></param>
        public WizardsViewModel(IModalService modalService, IWizardBuilder<NewWidgetModel> builder)
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
            IWizard wizard = Builder.NewWizard("New Widget")
                .WithModel(new NewWidgetModel())
                .AddStep<NewWidgetWizardStep>()
                .AddStep<NewWidgetSummaryStep>()
                .OnCancelled(modal => Task.FromResult<Result<bool>>(true))
                .OnFinished(model =>
                {
                    Widgets.Add(model);
                    return Task.FromResult<Result<bool>>(true);
                })
                .Build();

            await ModalService.ShowWizardAsync(wizard, new Dictionary<string, object>()
            {
                {nameof(ModalComponentBase.Width), (250, 1200)},
                {nameof(ModalComponentBase.Height), (250, 1200)}
            });
        }
    }

}