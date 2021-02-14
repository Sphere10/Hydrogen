Plugins

Plugins provide a way to dynamic add functionality to the application. 

## Plugin Model

| Interface     | Implementation | Responsibilities                                             |
| ------------- | -------------- | ------------------------------------------------------------ |
| IPlugin       | Plugin         | Top level container - contains one or more `IApp` . Provides a ConfigureServices method that will populate a given `IServicesCollection` with the plugin's dependencies / services. |
| IApp          | App            | An application block  - selectable in the User Interface, contains one or more `IAppBlock` |
| IAppBlock     | AppBlock       | Block menu - provides a side menu and has one or more `IAppBlockPage` shown in the menu |
| IAppBlockPage | AppBlockPage   | A Page - represents a Blazor page user interface that can be navigated to by the browser. Displayed in the IAppBlock menu. Must have a corresponding Blazor razor page with matching `Route` value. |

![image-20210212140645640](..\resources\Plugin-model-classes.png)

## Plugin Services

Plugin services are registered in the dependency injection container as part of the application startup. The plugin services are useful within different parts of the application. Default implementations of the plugin services are registered as singleton instances.

| Interface      | Implementation       | Responsibilities                                             |
| -------------- | -------------------- | ------------------------------------------------------------ |
| IPluginLocator | StaticPluginLocator  | `IPluginLocator` service is used to locate a collection of `Type` each of which must be an `IPlugin` implementation . `StaticPluginLocator` current implementation contains a list of plugins hardcoded. Other locating strategies might be from file systems or web APIs. |
| IPluginManager | DefaultPluginManager | `IPluginManager` maintains  the collection of Plugin instances available within the application. `ConfigureServices` method is used to populate the `IServiceCollection` from the Blazor runtime with services and dependencies from each plugin. This service is useful within the application to know what Plugins are loaded and available as well as register plugin dependencies with the runtime. |
| IAppManager    | DefaultAppManager    | `IAppManager` service provides access to the available `IApp` instances and the currently selected page. Changes to the selected app and app page are published as events for components to respond to changes. |

![image-20210212143854865](..\resources\Plugin-services.png)