using System;
using System.Threading.Tasks;
using Hydrogen;
using Hydrogen.DApp.Presentation2.UI.Wizard;

namespace Hydrogen.DApp.Presentation2.Logic.Wizard {

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