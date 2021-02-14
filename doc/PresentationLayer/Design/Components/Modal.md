# Modals

Modals are implemented using a single modal host, and dynamically setting its content and hiding/showing the main host component. `IModalService` is used from within a page/view to request a modal be shown. The outcome of the user's interaction with the modal is returned as the result including optional data.



## Components

| Component                        | Responsibility                                               |
| -------------------------------- | ------------------------------------------------------------ |
| ModalHost.razor                  | Single instance reusable modal host component. Initialized in `MainLayout`. Hosts a `ModalComponentBase` instance, coordinates the showing and hiding of the modal itself. |
| ModalComponent<TViewModel>.razor | Abstract base class for modal content being hosted by `ModalHost` . Extends `ModalComponentBase` adding ViewModel support for modal components. |
| ModalComponentBase.razor         | Abstract base class from which `ModalComponent<TViewModel>` extends. Provides a non-generic interface for modal host to use |
| WizardModal.razor                | A `ModalComponent<TViewModel>` implementation used to display `IWizard` instances. |
| ModalTemplate.razor              | A simple Blazor templated component providing a consistent layout for modal component implementations to use in their view definitions. |



## IModalService

`IModalService` is a view service that is used to display a modal and receive the results from the user's interactions with the modal. A singleton instance is used that contains a reference to the singular `ModalHost` component. On `MainLayout` render, the `IModalHost` service is initialized with the `ModalHost` component reference.

![image-20210215114521161](..\..\resources\Modal-service.png)

| Member                                                       | Responsibilities                                             |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| `void Initialize(ModalHost component);`                      | Initialize the modal service passing a reference to the modal component. Called once by `MainLayout` to obtain the component reference to the `ModalHost` component. |
| `Task<ModalResult> ShowAsync<T>(Dictionary<string, object>? parameters = null) where T : ModalComponentBase;` | Show a `ModalComponentBase` component `T`  in the modal host and wait for the result of users interactions. `parameters` parameter is used to pass parameters to the new `T` instance. `T` instance is initialized and view modal instance constructed. Returns once `ModalComponentBase` indicates its completion |
| `Task<ModalResult> ShowWizardAsync(IWizard wizard, Dictionary<string, object>? parameters = null);` | Convenience method to show an `IWizard` instance within a `WizardModal` component. Pass parameters to customize the `WizardModal` instance. |



## ModalResult

The result of awaiting `ShowAsync<T>` is a `ModalResult` instance. The `ResultType` enum is set by the hosted `ModalComponentBase` and should indicate the result of user's interaction with the modal. 

![image-20210215120103037](..\..\resources\Modal-result.png)