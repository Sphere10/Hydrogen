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
            List<Type> steps,
            TModel modal,
            Func<TModel, Task<Result<bool>>>? onFinish,
            Func<TModel, Task<Result<bool>>>? onCancel)
        {
            Steps = steps ?? throw new ArgumentNullException(nameof(steps));
            Title = title ?? throw new ArgumentNullException(nameof(title));
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

        private ILookup<StepUpdateType, Type>? Updates { get; set; }

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
        /// <param name="steps"> type of steps to be added</param>
        // HS: the step argument should be IEnumerable<WizardStepViewModel<TModel>> and logic updates many screens at once, not single
        public void UpdateSteps(StepUpdateType updateType, IEnumerable<Type> steps)
        {
            List<Type> types = steps.ToList();

            if (!StepUpdateIsApplied(updateType, types))
            {
                switch (updateType)
                {
                    case StepUpdateType.Inject:
                    {
                        Steps.InsertRangeSequentially(CurrentStepIndex + 1, types);
                        break;
                    }

                    case StepUpdateType.ReplaceAllNext:
                    {
                        if (HasNext)
                        {
                            Steps.RemoveRangeSequentially(CurrentStepIndex + 1, Steps.Count - CurrentStepIndex - 1);
                        }

                        Steps.InsertRangeSequentially(CurrentStepIndex + 1, types);

                        break;
                    }
                    case StepUpdateType.ReplaceAll:
                    {
                        Steps = new List<Type>(types);
                        CurrentStep = Steps[CurrentStepIndex];
                        break;
                    }
                    case StepUpdateType.RemoveNext:
                    {
                        foreach (Type type in types)
                        {
                            for (int i = CurrentStepIndex + 1; i < Steps.Count; i++)
                            {
                                if (Steps[i] == type)
                                {
                                    Steps.RemoveAt(i);
                                }
                            }
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(updateType), updateType, null);
                }
            }
        }

        private bool StepUpdateIsApplied(StepUpdateType type, IEnumerable<Type> steps)
        {
            if (Updates is null)
            {
                Updates = steps.ToLookup(x => type);
                return false;
            }
            else
            {
                return steps.All(x => Updates[type].Contains(x));
            }
        }
    }
}

