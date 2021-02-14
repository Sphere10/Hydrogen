# Navigation

Navigation within the Blazor framework is browser-URL based and the result output is directed by the router component and main layout. This system is utilized for navigating to pages supplied by `AppBlocks` from loaded `Plugins`. 

## App.razor + Router

App.razor is the Blazor root component containing the router component. The layout for this router is specified as a parameter (MainLayout.razor). 

The Blazor navigation system may route to pages located in assemblies other than the application's assemblies by specifying the AdditionalAssemblies property. This is supplied by the ViewModel and `IPluginLocator` service (Plugin assemblies are added to the additional available routing assemblies)

```c#
//App.Razor

@inject AppViewModel ViewModel

<Router AppAssembly="@typeof(Program).Assembly" PreferExactMatches="@true" AdditionalAssemblies="@ViewModel.RoutingAssemblies">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)"/>
    </Found>
    <NotFound>
        <RouteView RouteData="new RouteData(typeof(NotFound), new Dictionary<string, object>())" DefaultLayout="@typeof(MainLayout)"></RouteView>
    </NotFound>
</Router>
```



## Plugin Page Navigation

Navigation to pages provided by plugins is achieved through browser URL navigation and utilizing the `NavigationManager` abstract class from the Blazor framework. This process is Blazor first, the Hydrogen specific classes then observe changes via `NavigationManager`.

**Navigation Process:**

1. URL is navigated to by the browser, either from the sidebar or manually entered
2. Blazor Router component attempts to locate the page matching the URL. Framework checks main assembly and additional assembly razor pages for matching URL.
3. Found page is activated by Framework and displayed in Main Content section of Layout
4. `DefaultAppManager` observes `NavigationManager`'s `LocationChanged` event, updating its `SelectedPage` and `SelectedApp` properties and raises events `AppSelected` and `AppBlockPageSelected` 
5. Hydrogen specific view components such as sidebar respond to `IAppManager` events notifying them of change to page and app block.

```c#
// DefaultAppManager -- handling the location changed event.

/// <summary>
/// Handles the location changed event from nav manager. 
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
{
    SelectedApp = Apps.SingleOrDefault(x => x.Route ==
        NavigationManager
            .ToBaseRelativePathWithSlash(NavigationManager.Uri)
            .ToAppPathFromBaseRelativePath());

    SelectedPage = SelectedApp?.AppBlocks
        .SelectMany(x => x.AppBlockPages)
        .SingleOrDefault(x => x.Route == NavigationManager.ToBaseRelativePathWithSlash(NavigationManager.Uri)
            .TrimQueryParameters());
    
    if (SelectedApp is not null)
    {
        AppSelected?.Invoke(this, new AppSelectedEventArgs(SelectedApp));
    }

    if (SelectedPage is not null)
    {
        AppBlockPageSelected?.Invoke(this, new AppBlockPageSelectedEventArgs(SelectedPage));
    }
}
```