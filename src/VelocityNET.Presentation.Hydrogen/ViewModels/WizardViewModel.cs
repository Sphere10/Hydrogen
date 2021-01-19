using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{

    public class WizardViewModel<TModel> : ComponentViewModelBase
    {
        /// <summary>
        /// Raised when the wizard is finished.
        /// </summary>
        public event EventHandler? Finished;

        /// <summary>
        /// Gets or sets the list of step types. The order of these objects is the order
        /// they will appear in the wizard.
        /// </summary>
        public IEnumerable<Type>? Steps
        {
            get => StepList;
            set
            {
                if (value is not null && value.Any())
                {
                    StepList = new LinkedList<Type>(value);
                    CurrentStep = StepList.First!;
                }
            }
        }

        /// <summary>
        /// Gets the internal linked list of step types.
        /// </summary>
        private LinkedList<Type> StepList { get; set; } = new();

        /// <summary>
        /// Current step backing field
        /// </summary>
        private LinkedListNode<Type> _currentStep = null!;

        /// <summary>
        /// Gets or sets the current step of the wizard
        /// </summary>
        public LinkedListNode<Type> CurrentStep
        {
            get => _currentStep;
            private set
            {
                _currentStep = value;
                CurrentStepFragment = CreateFragmentOfType(value.Value);
            }
        }

        /// <summary>
        /// Gets or sets the current step as render fragment.
        /// </summary>
        public RenderFragment CurrentStepFragment { get; private set; } = null!;

        /// <summary>
        /// Move to the next step in the wizard.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"> thrown if there is no next step</exception>
        public async Task NextAsync()
        {
            if (HasNext)
            {
                if (await CurrentStepInstance.OnNextAsync())
                {
                    CurrentStep = CurrentStep.Next!;
                }
            }
            else
            {
                throw new InvalidOperationException("No next step");
            }
        }

        /// <summary>
        /// Move to previous step in wizard
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"> thrown if there is no previous</exception>
        public async Task PreviousAsync()
        {
            if (!HasPrevious || IsFinished)
            {
                throw new InvalidOperationException("Wizard cannot move to previous step.");
            }
            
            if (await CurrentStepInstance.OnPreviousAsync())
            {
                CurrentStep = CurrentStep.Previous!;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is a next step in the wizard.
        /// </summary>
        public bool HasNext => CurrentStep.Next is not null;

        /// <summary>
        /// Gets a value indicating whether there is a previous step in the wizard
        /// </summary>
        public bool HasPrevious => CurrentStep.Previous is not null;

        /// <summary>
        /// Gets or sets a value indicating whether the wizard has finished.
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// Gets or sets the model object passed to steps.
        /// </summary>
        public TModel Model { get; set; }

        /// <summary>
        /// Finish the wizard workflow. 
        /// </summary>
        /// <returns></returns>
        public Task FinishAsync()
        {
            IsFinished = true;
            Finished?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the reference to the step instance being rendered.
        /// </summary>
        public WizardStepBase CurrentStepInstance { get; set; } = null!;

        /// <summary>
        /// Create render fragment of wizard step type.
        /// </summary>
        /// <param name="componentType"> type of step</param>
        /// <returns></returns>
        private RenderFragment CreateFragmentOfType(Type componentType)
        {
            return builder =>
            {
                builder.OpenComponent(0, componentType);
                builder.AddComponentReferenceCapture(0, o => CurrentStepInstance = (WizardStepBase) o);
                builder.CloseComponent();
            };
        }
    }

}