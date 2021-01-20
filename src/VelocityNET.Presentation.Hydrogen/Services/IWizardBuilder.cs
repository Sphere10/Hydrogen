using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;

namespace VelocityNET.Presentation.Hydrogen.Services
{

    /// <summary>
    /// Wizard builder
    /// </summary>
    public interface IWizardBuilder
    {
        IWizardBuilder NewWizard<TWizard>() where TWizard : Wizard;

        IWizardBuilder WithModel<TModel>(TModel instance);

        IWizardBuilder AddStep<TWizardStep>() where TWizardStep : WizardStepBase;

        RenderFragment Build();
    }
}