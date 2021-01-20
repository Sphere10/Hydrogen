using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;

namespace VelocityNET.Presentation.Hydrogen.Services
{

    /// <summary>
    /// Wizard builder - constructs wizard component and produces render fragment delegate
    /// to be used with view / component.
    /// </summary>
    public class DefaultWizardBuilder : IWizardBuilder
    {
        /// <summary>
        /// Gets or sets the wizard type being built.
        /// </summary>
        private Type? Wizard { get; set; }

        /// <summary>
        /// Gets or sets the model 
        /// </summary>
        private object? Model { get; set; }

        /// <summary>
        /// Gets or sets the on finished func[
        /// </summary>
        private Func<object, Task>? OnFinshed { get; set; }

        /// <summary>
        /// Gets or sets the wizard steps
        /// </summary>
        private List<Type> Steps { get; } = new();

        /// <summary>
        /// Add wizard type to builder
        /// </summary>
        /// <typeparam name="TWizard"> type of wizard to build</typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"> if called more than once</exception>
        public IWizardBuilder NewWizard<TWizard>() where TWizard : Wizard
        {
            Wizard = typeof(TWizard);
            return this;
        }

        /// <summary>
        /// Set the model instance to be used with this wizard
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IWizardBuilder WithModel<TModel>(TModel instance)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance), "Model instance must not be null.");
            }

            Model = instance;
            return this;
        }

        /// <summary>
        /// Add step to the wizard
        /// </summary>
        /// <typeparam name="TWizardStep"></typeparam>
        /// <returns></returns>
        public IWizardBuilder AddStep<TWizardStep>() where TWizardStep : WizardStepBase
        {
            Steps.Add(typeof(TWizardStep));
            return this;
        }

        /// <summary>
        /// Optionally add handler to receive the model back once the wizard is finished.
        /// </summary>
        /// <param name="onFinishedHandler"> delegate</param>
        /// <returns> builder</returns>
        public IWizardBuilder OnFinished(Func<object, Task> onFinishedHandler)
        {
            OnFinshed = onFinishedHandler;
            return this;
        }

        /// <summary>
         /// Build the wizard render fragment
         /// </summary>
         /// <returns></returns>
         /// <exception cref="InvalidOperationException"> thrown if components of the wizard have not been added using builder.</exception>
        public RenderFragment Build()
        {
            if (Model is null)
            {
                throw new InvalidOperationException("Model has not been set, use WithModel<TModel>(TModel instance)");
            }

            if (!Steps.Any())
            {
                throw new InvalidOperationException(
                    "Steps have not been added to the wizard, at least one step required");
            }

            if (Wizard is null)
            {
                throw new InvalidOperationException("Wizard type is required");
            }

            RenderFragment frag =  builder =>
            {
                builder.OpenComponent(0, Wizard);
                builder.AddAttribute(0, "Steps", Steps);
                builder.AddAttribute(0, "Model", Model);

                if (OnFinshed is not null)
                {
                    builder.AddAttribute(0, "OnFinished", OnFinshed);
                }

                builder.CloseComponent();
            };

            return frag;
        }
    }
}