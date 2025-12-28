# MainLayout

A main layout is required when using the Blazor router component. The main layout is a template for the router outlet. The main layout template is a razor markup file and is composed of multiple components for elements within the UI layout. 

## Components

**Sidebar**

The Sidebar component makes up the left hand collapsible side menu. This component is composed of smaller components:

- SidebarBrand: The logo brand section of the sidebar, with dropdown for selecting the node. Uses `IEndpointManager` to get and set the active node endpoint in use. 
- BlockMenu: Displays a collapse menu for each "block" within the selected "app block". Menu items navigate to pages within the block.
- AppsMenu: The fixed position fixed height bottom section of the side menu. Displays the icon of each of the available "App Blocks" from loaded plugins. Selecting an App Block from this menu updates the block menu accordingly.

**Topbar**

The topbar component is the top most navigation section and contains sub components:

- Search: Templated search component
- Account: Placeholder account management section

**Main Menu**

The main menu component is the second most top navigation section. This menu is populated by the selected 'page' from the app blocks' block. The application may provide a default list of navigation items also, to which the default set are merged with.*

## 

## Main Content

The MainLayout serves as the template for the router component's output. The '@Body' directive is used within the MainLayout to declare where the routed page's content will be rendered.

Main content shows the navigated / routed page's content and is updated during navigation actions.

