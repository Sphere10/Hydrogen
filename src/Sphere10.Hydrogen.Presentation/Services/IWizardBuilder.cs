using System;
using System.Threading.Tasks;
using Sphere10.Framework;
using Sphere10.Hydrogen.Presentation.Components.Wizard;

namespace Sphere10.Hydrogen.Presentation.Services {

    /// <summary>
    /// Wizard builder
    /// </summary>
    public interface IWizardBuilder<TModel> {
        IWizardBuilder<TModel> NewWizard(string title);

        IWizardBuilder<TModel> WithModel(TModel instance);

        IWizardBuilder<TModel> AddStep<TWizardStep>() where TWizardStep : WizardStepBase;

        IWizardBuilder<TModel> OnFinished(Func<TModel, Task<Result<bool>>> onFinished);

        IWizardBuilder<TModel> OnCancelled(Func<TModel, Task<Result<bool>>> onCancelled);

        IWizard<TModel> Build();
    }
}