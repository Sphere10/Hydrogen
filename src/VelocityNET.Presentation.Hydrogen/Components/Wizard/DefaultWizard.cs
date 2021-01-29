using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sphere10.Framework;

namespace VelocityNET.Presentation.Hydrogen.Components.Wizard
{

    public class DefaultWizard<TModel> : IWizard<TModel>
    {
        public DefaultWizard(
            string title,
            IEnumerable<Type> steps,
            TModel modal,
            Func<TModel, Task<Result<bool>>>? onFinish,
            Func<TModel, Task<Result<bool>>>? onCancel)
        {
            if (steps is null)
            {
                throw new ArgumentNullException(nameof(steps));
            }

            Steps = steps is IList<Type> list ? list : new List<Type>(steps);
            Title = title;
            Model = modal ?? throw new ArgumentNullException(nameof(modal));
            OnFinish = onFinish;
            OnCancel = onCancel;

            if (!Steps.Any())
            {
                throw new ArgumentException("One or steps are required", nameof(steps));
            }

            CurrentStep = Steps[CurrentStepIndex];
        }

        private int CurrentStepIndex { get; set; }

        private IList<Type> Steps { get; set; }

        private Func<TModel, Task<Result<bool>>>? OnFinish { get; }

        private Func<TModel, Task<Result<bool>>>? OnCancel { get; }

        public string Title { get; }

        public Type CurrentStep { get; private set; }

        public TModel Model { get; }

        public bool HasNext => CurrentStepIndex < Steps.Count - 1;

        public bool HasPrevious => CurrentStepIndex > 0;

        /// <inheritdoc />
        public Result<bool> Next()
        {
            if (HasNext)
            {
                CurrentStepIndex++;
                CurrentStep = Steps[CurrentStepIndex];
                return new Result<bool>(true);
            }
            else
            {
                return Result<bool>.Error("No next step.");
            }
        }

        /// <inheritdoc />
        public Result<bool> Previous()
        {
            if (HasPrevious)
            {
                CurrentStepIndex--;
                CurrentStep = Steps[CurrentStepIndex];
                return new Result<bool>(true);
            }
            else
            {
                return Result<bool>.Error("No previous step");
            }
        }

        /// <inheritdoc />
        public async Task<Result<bool>> FinishAsync()
        {
            return OnFinish is not null ? await OnFinish.Invoke(Model) : true;
        }

        /// <inheritdoc />
        public async Task<Result<bool>> CancelAsync()
        {
            return OnCancel is not null ? await OnCancel.Invoke(Model) : true;
        }

        /// <summary>
        /// Update the steps of the wizard with one or more new steps.
        /// </summary>
        /// <param name="updateType"> type of operation to perform when updating the steps</param>
        /// <param name="step"> type of step to be added</param>
        // HS: the step argument should be IEnumerable<WizardStepViewModel<TModel>> and logic updates many screens at once, not single
        public void UpdateSteps(StepUpdateType updateType, Type step)
        {
            switch (updateType)
            {
                case StepUpdateType.Inject:
                {
                    if (HasNext)
                    {
                        if (Steps[CurrentStepIndex + 1] != step)
                        {
                            Steps.Insert(CurrentStepIndex + 1, step);
                        }
                    }

                    break;
                }
                case StepUpdateType.ReplaceAllNext:
                {
                    Steps.RemoveRangeSequentially(CurrentStepIndex + 1, Steps.Count - CurrentStepIndex);
                    Steps.Insert(CurrentStepIndex + 1, step);
                    break;
                }
                case StepUpdateType.ReplaceAll:
                {
                    Steps = new List<Type>
                    {
                        step
                    };

                    CurrentStepIndex = 0;

                    break;
                }
                case StepUpdateType.RemoveNext:
                {
                    if (HasNext)
                    {
                        Steps.RemoveAt(CurrentStepIndex + 1);
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(updateType), updateType, null);
            }
        }
    }

}