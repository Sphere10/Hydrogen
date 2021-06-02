# Dependency Injection

Blazor is based around .NET Core dependency injection and the Microsoft `IServicesCollection` + `IServiceProvider` container. Using this main container, dependencies can be provided automatically to views as they're rendered. 

## Framework Services

Framework services are registered in `Sphere10.Hydrogen.Presentation.Loader` `Program.cs` as part of the `Host` build up process. 



## View Model registration

View Model automatic registration is supported through a naming convention-based type scanning process. A extension method to `IServiceCollection` is available in the `Sphere10.Hydrogen.Presentation ` namespace:

```c#
public static IServiceCollection AddViewModelsFromAssembly(this IServiceCollection serviceCollection,
    Assembly assembly)
```

public non-abstract classes whose name contains "ViewModel" will automatically be registered in the applications `IServiceCollection`. 



## Plugin Dependencies

`Plugin` implementations must provide an implementation of `ConfigureServicesInternal`. At runtime `IPluginManager` will use this implementation to add the plugin's dependencies to the main application `IServiceCollection`

```c#
/// <summary>
/// Extension point for plugins to configure required services.
/// </summary>
/// <param name="serviceCollection"> service collection</param>
protected abstract void ConfigureServicesInternal(IServiceCol
```

See [Plugins](./Plugins.md) for more information. 