﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{

    public class WizardHostViewModel : ComponentViewModelBase
    {
        private RenderFragment? _currentStep;

        private WizardStepBase? _currentStepInstance;

        /// <summary>
        /// Gets a list of error messages zzs
        /// </summary>
        public List<string> ErrorMessages { get; } = new();

        /// <summary>
        /// Gets or sets the wizard model
        /// </summary>
        public IWizard Wizard { get; set; }

        /// <summary>
        /// Gets or sets the title of the current wizard and step.
        /// </summary>
        public string Title => $"{Wizard.Title} -> {CurrentStepInstance?.Title}";

        /// <summary>
        /// Gets or sets the callback function supplied by parent to be run when
        /// the wizard has finished.
        /// </summary>
        public EventCallback OnFinished { get; set; }

        /// <summary>
        /// Gets or sets the callback function supplied by parent to be run when
        /// the wizard has finished.
        /// </summary>
        public EventCallback OnCancelled { get; set; }

        /// <summary>
        /// Gets or sets the component ref instance of the current step.
        /// </summary>
        public WizardStepBase? CurrentStepInstance
        {
            get => _currentStepInstance;
            private set
            {
                _currentStepInstance = value;
                StateHasChangedDelegate?.Invoke();
            }
        }

        /// <summary>
        /// Gets or sets the current render fragment representation of the current step.
        /// </summary>
        public RenderFragment? CurrentStep
        {
            get => _currentStep;
            private set
            {
                _currentStep = value;
                StateHasChangedDelegate?.Invoke();
            }
        }

        /// <summary>
        /// Move to the next step in the wizard.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"> thrown if there is no next step</exception>
        public void Next()
        {
            Result<bool> result = Wizard.Next();

            if (result)
            {
                CurrentStep = CreateStepBaseFragment(Wizard.CurrentStep);
            }
            else
            {
                ErrorMessages.Clear();
                ErrorMessages.AddRange(result.ErrorMessages);
            }
        }

        /// <summary>
        /// Move to previous step in wizard
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"> thrown if there is no previous</exception>
        public void Previous()
        {
            Result<bool> result = Wizard.Previous();

            if (result)
            {
                CurrentStep = CreateStepBaseFragment(Wizard.CurrentStep);
            }
            else
            {
                ErrorMessages.Clear();
                ErrorMessages.AddRange(result.ErrorMessages);
            }
        }

        /// <summary>
        /// Finish the wizard workflow. 
        /// </summary>
        /// <returns></returns>
        public async Task FinishAsync()
        {
            Result result = await Wizard.FinishAsync();

            if (result.Success)
            {
                await OnFinished.InvokeAsync();
            }
            else
            {
                ErrorMessages.Clear();
                ErrorMessages.AddRange(result.ErrorMessages);
            }
        }

        /// <summary>
        /// Cancel the wizard workflow
        /// </summary>
        /// <returns></returns>
        public async Task CancelAsync()
        {
            Result result = await Wizard.CancelAsync();

            if (result.Success)
            {
                await OnCancelled.InvokeAsync();
            }
            else
            {
                ErrorMessages.Clear();
                ErrorMessages.AddRange(result.ErrorMessages);
            }
        }

        /// <inheritdoc />
        protected override Task InitCoreAsync()
        {
            CurrentStep = CreateStepBaseFragment(Wizard.CurrentStep);
            return base.InitCoreAsync();
        }

        /// <summary>
        /// Create render fragment of wizard step type.
        /// </summary>
        /// <param name="componentType"> type of step</param>
        /// <returns></returns>
        private RenderFragment CreateStepBaseFragment(Type componentType)
        {
            return builder =>
            {
                int index = 0;

                builder.OpenComponent(index, componentType);
                builder.AddAttribute(index++, nameof(Wizard), Wizard);
                builder.AddComponentReferenceCapture(index++, o => CurrentStepInstance = (WizardStepBase) o);
                builder.CloseComponent();
            };
        }
    }

}