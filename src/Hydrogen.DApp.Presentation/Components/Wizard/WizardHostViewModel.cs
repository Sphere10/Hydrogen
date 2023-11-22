// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Hydrogen.DApp.Presentation.ViewModels;

namespace Hydrogen.DApp.Presentation.Components.Wizard;

public class WizardHostViewModel : ComponentViewModelBase {
	private WizardStepBase? _currentStepInstance;

	/// <summary>
	/// Gets a list of error messages zzs
	/// </summary>
	public List<string> ErrorMessages { get; } = new();

	/// <summary>
	/// Gets or sets the wizard model
	/// </summary>
	public IWizard Wizard { get; set; } = null!;

	/// <summary>
	/// Gets or sets the title of the current wizard and step.
	/// </summary>
	public string Title { get; private set; } = null!;

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
	public WizardStepBase? CurrentStepInstance {
		get => _currentStepInstance;
		private set {
			_currentStepInstance = value;
			Title = $"{Wizard.Title} -> {_currentStepInstance?.Title}";
			StateHasChangedDelegate?.Invoke();
			OnStepChange.InvokeAsync();
		}
	}

	/// <summary>
	/// Gets or sets the current render fragment representation of the current step.
	/// </summary>
	public RenderFragment? CurrentStep { get; private set; }

	/// <summary>
	/// Gets or sets the on step change event callback.
	/// </summary>
	public EventCallback OnStepChange { get; set; }

	/// <summary>
	/// Move to the next step in the wizard.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"> thrown if there is no next step</exception>
	public async Task NextAsync() {
		Result result = await _currentStepInstance!.OnNextAsync();
		ErrorMessages.Clear();

		if (result.IsSuccess) {
			if (Wizard.Next()) {
				CurrentStep = CreateStepBaseFragment(Wizard.CurrentStep);
			}
		} else {
			ErrorMessages.AddRange(result.ErrorMessages);
		}
	}

	/// <summary>
	/// Move to previous step in wizard
	/// </summary>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"> thrown if there is no previous</exception>
	public Task PreviousAsync() {
		var prev = Wizard.Previous();
		ErrorMessages.Clear();

		if (prev) {
			CurrentStep = CreateStepBaseFragment(Wizard.CurrentStep);
		} else {
			ErrorMessages.AddRange(prev.ErrorMessages);
		}

		return Task.CompletedTask;
	}

	/// <summary>
	/// Finish the wizard workflow. 
	/// </summary>
	/// <returns></returns>
	public async Task FinishAsync() {
		Result result = await Wizard.FinishAsync();
		ErrorMessages.Clear();

		if (result.IsSuccess) {
			await OnFinished.InvokeAsync();
		} else {
			ErrorMessages.AddRange(result.ErrorMessages);
		}
	}

	/// <summary>
	/// Cancel the wizard workflow
	/// </summary>
	/// <returns></returns>
	public async Task CancelAsync() {
		Result result = await Wizard.CancelAsync();
		ErrorMessages.Clear();

		if (result.IsSuccess) {
			await OnCancelled.InvokeAsync();
		} else {
			ErrorMessages.AddRange(result.ErrorMessages);
		}
	}

	/// <inheritdoc />
	protected override Task InitCoreAsync() {
		CurrentStep = CreateStepBaseFragment(Wizard.CurrentStep);
		return base.InitCoreAsync();
	}

	/// <summary>
	/// Create render fragment of wizard step type.
	/// </summary>
	/// <param name="componentType"> type of step</param>
	/// <returns></returns>
	private RenderFragment CreateStepBaseFragment(Type componentType) {
		return builder => {
			int index = 0;

			builder.OpenComponent(index, componentType);
			builder.AddAttribute(index++, nameof(Wizard), Wizard);
			builder.AddComponentReferenceCapture(index++, o => CurrentStepInstance = (WizardStepBase)o);
			builder.CloseComponent();
		};
	}
}
