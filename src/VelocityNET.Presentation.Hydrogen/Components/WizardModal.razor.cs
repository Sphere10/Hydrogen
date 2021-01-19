using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace VelocityNET.Presentation.Hydrogen.Components
{
    /// <summary>
    /// Wizard modal - show a wizard component inside a modal dialog.
    /// </summary>
    /// <typeparam name="TWizard"> wizard type</typeparam>
    public partial class WizardModal<TWizard> where TWizard : ComponentBase
    {
        /// <summary>
        /// Gets or sets the model object to be passed to the wizard.
        /// </summary>
        [Parameter]
        public object Model { get; set; }
        
        /// <summary>
        /// Gets or sets the modal title
        /// </summary>
        [Parameter]
        public string Title { get; set; } = "Wizard";
        
        /// <summary>
        /// Gets or sets the wizard steps.
        /// </summary>
        [Parameter]
        public IEnumerable<Type> Steps { get; set; }

        /// <summary>
        /// Gets or sets the wizard component reference instance.
        /// </summary>
        private TWizard ComponentInstance { get; set; }
        
        /// <summary>
        /// Gets or sets the wizard render fragment
        /// </summary>
        private RenderFragment Wizard { get; set; }

        /// <inheritdoc />
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

                Wizard = builder =>
                {
                    builder.OpenComponent<TWizard>(0);
                    builder.AddAttribute(0, "OnFinished", EventCallback.Factory.Create(ViewModel, () => ViewModel!.OkData(Model)));
                    builder.AddAttribute(0, "Steps", Steps);
                    builder.AddAttribute(0, "Model", Model);
                    builder.AddComponentReferenceCapture(0, o => ComponentInstance = (TWizard) o);
                    builder.CloseComponent();
                };
       }
    }
}