using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Components.Wizard
{

    /// <summary>
    /// Wizard component.
    /// </summary>
    /// <typeparam name="TModel"> model type</typeparam>
    public partial class Wizard<TModel> : IDisposable
    {
        /// <summary>
        /// Call back, invoked when wizard is finished. signals to parent components the wizard
        /// is complete.
        /// </summary>
        [Parameter] 
        public EventCallback OnFinished { get; set; }

        /// <summary>
        /// Gets or sets the collection of step types. The order of these objects reflects the order
        /// of the steps in the wizard.
        /// </summary>
        [Parameter]
        public IEnumerable<Type> Steps
        {
            get => ViewModel!.Steps!;
            set => ViewModel!.Steps = value;
        }

        /// <inheritdoc />
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                ViewModel!.Finished += OnWizardFinished;
            }
        }

        /// <summary>
        /// Handles the wizard finished event, invokes the provided delegate parameter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnWizardFinished(object? sender, EventArgs args)
        {
            OnFinished.InvokeAsync();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            ViewModel!.Finished -= OnWizardFinished;
        }
    }

    /// <summary>
    /// Wizard component base
    /// </summary>
    /// <typeparam name="TModel"> model type</typeparam>
    /// <typeparam name="TViewModel"> view model type</typeparam>
    public abstract class WizardComponentBase<TModel, TViewModel> : ComponentWithViewModel<TViewModel>
        where TViewModel : WizardViewModel<TModel>
    {
        [Parameter]
        public TModel Model
        {
            get => ViewModel!.Model;
            set => ViewModel!.Model = value;
        }
    }
}