using System.Collections.Generic;
using System.Threading.Tasks;
using Sphere10.Framework;
using Sphere10.Hydrogen.Presentation.Components.Wizard;
using Sphere10.Hydrogen.Presentation.Services;
using Sphere10.Hydrogen.Presentation.ViewModels;
using Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.Components;
using Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.Models;

namespace Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.ViewModels {

    public class WizardsViewModel : ComponentViewModelBase {
        /// <summary>
        /// Gets the wizard builder.
        /// </summary>
        private IWizardBuilder<NewWidgetModel> Builder { get; }

        /// <summary>
        /// Gets list of widgets 
        /// </summary>
        public List<NewWidgetModel> Widgets { get; } = new();

        /// <summary>
        /// Wizards view model
        /// </summary>
        /// <param name="builder"></param>
        public WizardsViewModel(IWizardBuilder<NewWidgetModel> builder) {
            Builder = builder;
        }

        /// <summary>
        /// Creates a new instance of the wizard model.
        /// </summary>
        /// <returns> new wizard model insteance</returns>
        public IWizard NewWidetWizard() {
            IWizard wizard = Builder.NewWizard("New Widget")
                .WithModel(new NewWidgetModel())
                .AddStep<NewWidgetWizardStep>()
                .AddStep<NewWidgetSummaryStep>()
                .OnCancelled(modal => {
                    var result = new Result<bool>(false);
                    result.AddError("Cancel not allowed!");
                    return Task.FromResult(result);
                })
                .OnFinished(model => {
                    Widgets.Add(model);
                    return Task.FromResult<Result<bool>>(true);
                })
                .Build();

            return wizard;
        }
    }

}