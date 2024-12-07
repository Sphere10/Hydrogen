// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.DApp.Presentation2.Logic.Wizard {

	/// <summary>
	/// Default wizard
	/// </summary>
	/// <typeparam name="TModel"> model type</typeparam>
	public class DefaultWizard<TModel> : IWizard<TModel> {
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultWizard{TModel}"/> class.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="steps"></param>
		/// <param name="modal"></param>
		/// <param name="onFinish"></param>
		/// <param name="onCancel"></param>
		/// <param name="options"></param>
		/// <exception cref="ArgumentException"></exception>
		public DefaultWizard(
			string title,
			List<Type> steps,
			TModel modal,
			Func<TModel, Task<Result<bool>>> onFinish = null,
			Func<TModel, Task<Result<bool>>> onCancel = null,
			WizardOptions options = null) {
			Steps = steps ?? throw new ArgumentNullException(nameof(steps));
			Title = title ?? throw new ArgumentNullException(nameof(title));
			Model = modal ?? throw new ArgumentNullException(nameof(modal));

			OnFinish = onFinish;
			OnCancel = onCancel;

			Options = options ?? new WizardOptions();

			if (!Steps.Any()) {
				throw new ArgumentException("One or steps are required", nameof(steps));
			}

			CurrentStep = Steps[CurrentStepIndex];
		}

		/// <summary>
		/// Gets or sets the index of the step collection the wizard is currently at.
		/// </summary>
		private int CurrentStepIndex { get; set; }

		/// <summary>
		/// Gets or sets the step collection - types of steps that will be shown
		/// </summary>
		private List<Type> Steps { get; set; }

		/// <summary>
		/// Gets the on finished function to run when the wizard is finished.
		/// </summary>
		private Func<TModel, Task<Result<bool>>>? OnFinish { get; }

		/// <summary>
		/// Gets the on cancelled function to run when the wizard is cancelled.
		/// </summary>
		private Func<TModel, Task<Result<bool>>>? OnCancel { get; }

		/// <summary>
		/// Gets or sets the step update lookup used to track applied step updates
		/// </summary>
		private ILookup<StepUpdateType, Type>? Updates { get; set; }

		/// <summary>
		/// Gets the wizard title
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// Gets or sets the current step
		/// </summary>
		public Type CurrentStep { get; private set; }

		/// <summary>
		/// Gets the model
		/// </summary>
		public TModel Model { get; }

		/// <summary>
		/// Gets a value indicating whether there is a next step
		/// </summary>
		public bool HasNext => CurrentStepIndex < Steps.Count - 1;

		/// <summary>
		/// Gets or sets a value indicating whether there is a previous step.
		/// </summary>
		public bool HasPrevious => CurrentStepIndex > 0;

		/// <summary>
		/// Gets or sets the wizard options
		/// </summary>
		public WizardOptions Options { get; set; }

		/// <inheritdoc />
		public Result<bool> Next() {
			if (HasNext) {
				CurrentStepIndex++;
				CurrentStep = Steps[CurrentStepIndex];
				return new Result<bool>(true);
			} else {
				return Result<bool>.Error("No next step.");
			}
		}

		/// <inheritdoc />
		public Result<bool> Previous() {
			if (HasPrevious) {
				CurrentStepIndex--;
				CurrentStep = Steps[CurrentStepIndex];
				return new Result<bool>(true);
			} else {
				return Result<bool>.Error("No previous step");
			}
		}

		/// <inheritdoc />
		public async Task<Result<bool>> FinishAsync() {
			return OnFinish is not null ? await OnFinish.Invoke(Model) : true;
		}

		/// <inheritdoc />
		public async Task<Result<bool>> CancelAsync() {
			return OnCancel is not null ? await OnCancel.Invoke(Model) : true;
		}

		/// <summary>
		/// Update the steps of the wizard with one or more new steps.
		/// </summary>
		/// <param name="updateType"> type of operation to perform when updating the steps</param>
		/// <param name="steps"> type of steps to be added</param>
		// HS: the step argument should be IEnumerable<WizardStepViewModel<TModel>> and logic updates many screens at once, not single
		public void UpdateSteps(StepUpdateType updateType, IEnumerable<Type> steps) {
			List<Type> types = steps.ToList();

			switch (updateType) {
				case StepUpdateType.Inject: {
					if (Steps.GetRange(CurrentStepIndex + 1, types.Count) != types) {
						Steps.InsertRangeSequentially(CurrentStepIndex + 1, types);
					}

					break;
				}
				case StepUpdateType.ReplaceAllNext: {
					if (HasNext) {
						Steps.RemoveRangeSequentially(CurrentStepIndex + 1, Steps.Count - CurrentStepIndex - 1);
					}

					Steps.InsertRangeSequentially(CurrentStepIndex + 1, types);

					break;
				}
				case StepUpdateType.ReplaceAll: {
					Steps = new List<Type>(types);
					CurrentStep = Steps[CurrentStepIndex];
					break;
				}
				case StepUpdateType.RemoveNext: {
					foreach (Type type in types) {
						for (int i = CurrentStepIndex + 1; i < Steps.Count; i++) {
							if (Steps[i] == type) {
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
}
