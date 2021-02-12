# Plugins

Plugins provide a way to add functionality to the application in a dynamic manner. The registered `IPluginLocator` (`StaticPluginLocator` currently ) instance returns the `IPlugin` implementations to be loaded and available within the application.

## Plugin Model

- Plugin: Top level container - contains one or more `IApp` . Provides a ConfigureServices method that will populate a given `IServicesCollection` with the 

  plugin's services.

- App: An application - Selectable in the User Interface, contains one or more `IAppBlock`

- AppBlock: Application block - provides a side menu and has one or more `IAppBlockPage` shown in the menu

- AppBlockPage: A Page - represents a Blazor page user interface that can be navigated to by the browser

   ![image-20210212140645640](..\resources\Plugin-model-classes.png)

## Plugin Services

Plugin services are registered in the dependency injection container as part of the application startup. Default implementations of the plugin services are registered as singleton instances.

![image-20210212143854865](..\resources\Plugin-services.png)

**IPluginLocator**

`IPluginLocator` service is used to locate a collection of `Type` each of which must be an `IPlugin` implementation . `StaticPluginLocator` current implementation contains a list of plugins hardcoded. Other locating strategies might be from file systems or web APIs.

**IPluginManager**

`IPluginManager` maintains  the collection of Plugin instances available within the application. `ConfigureServices` method is used to populate the `IServiceCollection` from the Blazor runtime with services and dependencies from each plugin. This service is useful within the application to know what Plugins   are loaded and available as well as register plugin dependencies with the runtime.

`DefaultPluginManager` implementation uses `IPluginLocator` at startup and activates instances of each plugin. 

**IAppManager**

`IAppManager` service provides access to the available `IApp` instances and the currently selected page. Changes to the selected app and app page are published as events for components to respond to changes. 

`DefaultAppManager` implementation uses the `IPluginManager` to determine available apps. `NavigationManager` Blazor service is used to watch for changes in the browser's URL, and update the `SelectedApp` and `SelectedPage` accordingly. 

