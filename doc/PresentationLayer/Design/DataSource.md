# Data source

Hydrogen applications require a connection to the node data source, and this may be updated by the user through the user interface. `IEndpointManager` is responsible for keeping track of the currently selected endpoint, what endpoints are available and adding or removing endpoints. Application components and services may subscribe to `IEndpointManager`'s events to refresh their state after a change.

![image-20210215150043719](..\resources\IEndpointManager.png)

| Members                 | Comments                                     |
| ----------------------- | -------------------------------------------- |
| Endpoint                | The currently selected endpoint              |
| Endpoints               | Available endpoints                          |
| AddEndpointAsync        | Add new endpoint                             |
| SetCurrentEndpointAsync | Sets the current available endpoint          |
| ValidateEndpointAsync   | Validate an endpoint value                   |
| EndpointAdded           | Raised when a new endpoint is added          |
| EndpointChanged         | Raised when the selected endpoint is changed |



## Responding to changed endpoint

Some components that rely on data from the endpoint will want to reinitialize when the endpoint has changed. A proof of concept of how to achieve this in an MVVM style is `ExtendedComponentViewModel` abstract base class extending from `ComponentViewModelBase` 

![image-20210215150858201](..\resources\Extendedviewmodel.png)

`ExtendedComponentViewModel` is the base for page view models that want to automatically re-initialize on endpoint changes. A subscription is made to `IEndpointMangaer` `EndpointChanged` event, and InitAsync is triggered when the event occurs.

```c#

// ExtendedComponentViewModel IEndpoint event handler.

		/// <summary>
        /// Handles the endpoint changed event - reinitialize the view model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnEndpointChanged(object? sender, EventArgs e)
        {
            try
            {
                await InitCoreAsync();
            }
            catch (Exception exception)
            {
                // Do something, show error notification / message. 
                Console.WriteLine(exception);
            }
        }


```

This implementation relies on a single `IEndpointManager` service being used within the application. Another MVVM style approach would be to publish notification messagxes using a message bus to communicate to interested view models / components while maintaining loose coupling.