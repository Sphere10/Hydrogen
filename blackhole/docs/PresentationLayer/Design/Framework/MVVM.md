# MVVM framework

A simple, lightweight MVVM framework is implemented in a 'view first' style to extend the Blazor framework.

## Base classes

Abstract base classes for both view and view model's are used to establish the relationship between view and view model and provide an efficient developer workflow. 

![image-20210212113141653](..\..\resources\Mvvm-abstract-classes)

**ComponentWithViewModel** 

Extends blazor component base class `ComponentBase`, adds ViewModel property and requests ViewModel instance via Dependency Injection (Inject attribute). MVVM components such as controls and views should inherit from this class. `TViewModel` generic type parameter represents the type of view model that will be provided. TViewModel is constrained to implementations of ComponentViewModelBase.

On InitializedAsync from `ComponentBase` is overridden and used to initialize the view model. 

**ComponentViewModelBase**

View model base for view models of components/views that inherit `ComponentWithViewModel<TVIewModel>`. The view model base provides functionality useful for building view models.

- InitAsync: A public method invoked by the view to initialize the view model. Sets IsInitialized to true once complete.

- InitCoreAsync: A protected virtual method, View model implementations should overwrite to provide custom initialization

- IsInitialized: A bool property indicating whether the view model is initialized. Set internally via InitAsync. This is useful for showing loading indicators

  in the view while the view model is still initializing in the background. 

- StateHasChangedDelegate: A delegate set by the view. Invoking will inform the view the view model state has changed. 

**Initialization**

In a view first MVVM design, the application will determine the desired view to be shown, then pass it to the framework to be displayed. The view model for this view is then located, using Dependency Injection in this case.

A view implementing `ComponentWithViewModel` will call `InitAsync` of its view model during its initialization lifecycle. This allows for the view model to perform any setup required as the view is being shown. 

**StateHasChangedDelegate**

Blazor `ComponentBase` provides the method `StateHasChanged` which is used to notify the renderer that the view should be re-rendered. `ComponentWithViewModel` passes this method as a delegate to `ComponentViewModelBase` so that the ViewModel has a way to inform the view that it's state has changed (In a way that is not automatically detected)



