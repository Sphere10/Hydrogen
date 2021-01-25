using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Sphere10.Framework;

namespace VelocityNET.Presentation.Hydrogen.Components.Wizard
{

    /// <summary>
    /// Wizard component.
    /// </summary>
    public partial class Wizard
    {
        /// <summary>
        /// Raised when the wizard is cancelled.
        /// </summary>
        public event EventHandler? Cancelled;

        /// <summary>
        /// Raised when the wizard is finished.
        /// </summary>
        public event EventHandler? Finished;

        /// <summary>
        /// Call back, invoked when wizard is finished. cascaded from a parent component is used to signal
        /// the completion of the wizard.
        /// </summary>
        [CascadingParameter(Name = "OnFinished")]
        public EventCallback<object> OnFinished { get; set; }

        /// <summary>
        /// Call back, invoked when wizard is cancelled. cascaded from a parent component is used to signal
        /// the cancellation of the wizard.
        /// </summary>
        [CascadingParameter(Name = "OnCancelled")]
        public EventCallback OnCancelled { get; set; }

        /// <summary>
        /// Gets or sets the list of step types. The order of these objects is the order
        /// they will appear in the wizard.
        /// </summary>
        [Parameter]
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
        /// Gets or sets the model object
        /// </summary>
        [Parameter]
        public object Model { get; set; } = null!;

        /// <summary>
        /// Gets or sets the wizard title
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        /// <summary>
        /// Gets the title of the wizard, including the current step.
        /// </summary>
        private string WizardTitle =>
            Title is null ? CurrentStepInstance?.Title! : $"{Title!} -> {CurrentStepInstance?.Title!}";

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
        private LinkedListNode<Type> CurrentStep
        {
            get => _currentStep;
            set
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
        /// Current step instance backing field.
        /// </summary>
        private WizardStepBase? _currentStepInstance;

        /// <summary>
        /// Gets the reference to the step instance being rendered.
        /// </summary>
        private WizardStepBase? CurrentStepInstance
        {
            get => _currentStepInstance;
            set
            {
                _currentStepInstance = value;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Gets the model edit context.
        /// </summary>
        private EditContext EditContext { get; set; } = null!;

        /// <summary>
        /// Gets the validation message store 
        /// </summary>
        private ValidationMessageStore ValidationMessageStore { get; set; } = null!;

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Model is null)
            {
                throw new InvalidOperationException("Model parameter must be set on wizard");
            }

            EditContext = new EditContext(Model);
            ValidationMessageStore = new ValidationMessageStore(EditContext);

            EditContext.OnValidationRequested += (sender, _) =>
            {
                Result result = CurrentStepInstance!.Validate();
                ValidationMessageStore.Clear();

                if (result.Failure)
                {
                    foreach (string errorMessage in result.ErrorMessages)
                    {
                        ValidationMessageStore.Add(new FieldIdentifier(((EditContext) sender!).Model, string.Empty),
                            errorMessage!);
                    }
                }
            };
        }

        /// <summary>
        /// Move to the next step in the wizard.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"> thrown if there is no next step</exception>
        private async Task NextAsync()
        {
            if (HasNext)
            {
                bool isValid = EditContext.Validate();

                if (isValid)
                {
                    if (await CurrentStepInstance!.OnNextAsync())
                    {
                        if (CurrentStepInstance.HasNextStep)
                        {
                            StepList.AddAfter(CurrentStep, new LinkedListNode<Type>(CurrentStepInstance!.NextStep!));
                        }
                        
                        CurrentStep = CurrentStep.Next!;
                    }
                }
                else
                {
                    throw new InvalidOperationException("No next step");
                }
            }
        }

        /// <summary>
        /// Move to previous step in wizard
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"> thrown if there is no previous</exception>
        private async Task PreviousAsync()
        {
            if (!HasPrevious || IsFinished)
            {
                throw new InvalidOperationException("Wizard cannot move to previous step.");
            }

            if (await CurrentStepInstance!.OnPreviousAsync())
            {
                CurrentStep = CurrentStep.Previous!;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is a next step in the wizard.
        /// </summary>
        private bool HasNext => CurrentStep.Next is not null;

        /// <summary>
        /// Gets a value indicating whether there is a previous step in the wizard
        /// </summary>
        private bool HasPrevious => CurrentStep.Previous is not null;

        /// <summary>
        /// Gets or sets a value indicating whether the wizard has finished.
        /// </summary>
        private bool IsFinished { get; set; }

        /// <summary>
        /// Finish the wizard workflow. 
        /// </summary>
        /// <returns></returns>
        private Task FinishAsync()
        {
            Finished?.Invoke(this, EventArgs.Empty);
            IsFinished = true;
            OnFinished.InvokeAsync(Model);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Cancel the wizard workflow
        /// </summary>
        /// <returns></returns>
        private Task CancelAsync()
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
            OnCancelled.InvokeAsync();
            return Task.CompletedTask;
        }

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
                builder.AddAttribute(0, nameof(Model), Model);
                builder.AddComponentReferenceCapture(0, o => CurrentStepInstance = (WizardStepBase) o);
                builder.CloseComponent();
            };
        }
    }

}