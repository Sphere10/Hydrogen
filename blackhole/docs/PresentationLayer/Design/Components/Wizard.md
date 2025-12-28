# Wizard

Wizard components provide a way of creating a wizard style workflow which can be rendered in a modal or on a page. 

## Components

Wizard components also include View Model for control/component logic. 

| Component                            | Responsibilities                                             |
| ------------------------------------ | ------------------------------------------------------------ |
| WizardHost.razor                     | Hosts a wizard model instance. Provides the controls for navigating between wizard steps, a validation message summary and renders the current step of the `IWizard` model. <br />WizardHost requires parameters `IWizard` Wizard |
| WizardStep<TModel, TViewModel>.razor | Generic abstract base class for wizard steps. Provides a consistent contract that wizard steps will have to enable the wizard host to interact with generic wizard steps. TModel type parameter is the model object supplied to the wizard that is used to hold data collected during wizard steps. |
| WizardStepBase.razor                 | Non-generic abstract base class for wizard steps. Has virtual properties to allow the step to customize button text and title |



## IWizard

IWizard is the model for the wizard, and is a required parameter of the `WizardHost` component. Important to note that steps are referred to by their type, and when the step is required to be rendered an instance of the type is instantiated and rendered by `WizardHost` and the Blazor framework. Step types are required to be derived from `WizardStep<TModel, TViewModel>`

![image-20210215133003132](..\..\resources\IWizard.png)

## IWizardBuilder<TModel>

`IWizardBuilder<TModel> ` is used to create `IWIzard<TModel>` instances. A fluent method-chaining API is provided. Calling `Build` will produce an `IWizard`  model for use with `WizardHost` view component.

![image-20210215134255889](..\..\resources\IWIzardBuilder.png)

