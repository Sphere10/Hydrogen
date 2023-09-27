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
using Hydrogen.DApp.Presentation.Components.Wizard;
using Hydrogen.DApp.Presentation.Services;

namespace Hydrogen.DApp.Presentation.Loader.Services;

/// <summary>
/// Wizard builder - constructs wizard component and produces render fragment delegate
/// to be used with view / component.
/// </summary>
public class DefaultWizardBuilder<TModel> : IWizardBuilder<TModel> {
	private WizardBuilderParameters<TModel> _parameters = null!;

	/// <summary>
	/// Add wizard type to builder
	/// </summary>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"> if called more than once</exception>
	public IWizardBuilder<TModel> NewWizard(string title) {
		_parameters = new WizardBuilderParameters<TModel>() {
			Title = title
		};

		return this;
	}


	/// <summary>
	/// Set the model instance to be used with this wizard
	/// </summary>
	/// <param name="instance"></param>
	/// <typeparam name="TModel"></typeparam>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public IWizardBuilder<TModel> WithModel(TModel instance) {
		if (instance is null) {
			throw new ArgumentNullException(nameof(instance), "Model instance must not be null.");
		}

		_parameters.Model = instance;
		return this;
	}

	/// <summary>
	/// Add step to the wizard
	/// </summary>
	/// <typeparam name="TWizardStep"></typeparam>
	/// <returns></returns>
	public IWizardBuilder<TModel> AddStep<TWizardStep>() where TWizardStep : WizardStepBase {
		_parameters.Steps.Add(typeof(TWizardStep));
		return this;
	}

	public IWizardBuilder<TModel> OnFinished(Func<TModel, Task<Result<bool>>> onFinished) {
		_parameters.OnFinishedFunc = onFinished ?? throw new ArgumentNullException(nameof(onFinished));
		return this;
	}

	public IWizardBuilder<TModel> OnCancelled(Func<TModel, Task<Result<bool>>> onCancelled) {
		_parameters.OnCancelledFunc = onCancelled ?? throw new ArgumentNullException(nameof(onCancelled));
		return this;
	}

	/// <summary>
	/// Build the wizard render fragment
	/// </summary>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"> thrown if components of the wizard have not been added using builder.</exception>
	public IWizard<TModel> Build() {
		if (_parameters.Model is null) {
			throw new InvalidOperationException("Model has not been set, use WithModel<TModel>(TModel instance)");
		}

		if (!_parameters.Steps.Any()) {
			throw new InvalidOperationException(
				"Steps have not been added to the wizard, at least one step required");
		}

		return new DefaultWizard<TModel>(_parameters.Title!, _parameters.Steps, _parameters.Model, _parameters.OnFinishedFunc, _parameters.OnCancelledFunc);
	}
}


internal class WizardBuilderParameters<TModel> {
	/// <summary>
	/// Gets or sets the wizard title
	/// </summary>
	internal string? Title { get; set; }

	/// <summary>
	/// Gets or sets the model 
	/// </summary>
	internal TModel? Model { get; set; }

	/// <summary>
	/// Gets or sets the on finished func
	/// </summary>
	internal Func<TModel, Task<Result<bool>>>? OnFinishedFunc { get; set; }

	/// <summary>
	/// Gets or sets the cancelled func.
	/// </summary>
	internal Func<TModel, Task<Result<bool>>>? OnCancelledFunc { get; set; }

	/// <summary>
	/// Gets or sets the wizard steps
	/// </summary>
	internal List<Type> Steps { get; } = new();
}
