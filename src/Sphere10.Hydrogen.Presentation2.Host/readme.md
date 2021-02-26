# Loading Sequence

Actors:
- Hydrogen Loader
- Hydrogen Node
- Hydrogen UI Host (Blazor Host)
- Hydrogen UI


1. There exists a fixed Hydrogen.Loader which lives in Program Files (on Windows).
- Is a console application 
- A light-weight program
- Manages the lifecycle of the Hydrogen application it is responsible for
	- Initial deployment
	- Subsequent upgrades
	- Restarting

- When installed, it has a default Hydrogen application package which it deploys.
- Uses an INI file to determine the location of the deployment
	Example: %LocalApplicationData%/.VelocityNet/app

2. The Hydrogen Application 
 - lives in a user-controlled and read/write accesible folder
 - It contains assemblies for the node and the blazor ui
 - It contains the wwwroot for the blazorui
 - It contains the assemblies for the plugins which extend the application
 - It contais the folder that manages upgrades

 Example folder structure:
    Let approot = C:\Users\MyUser\LocalApplicationData\VelocityNet\

	%approot%\app                        ; lives the console node
	%approot%\app\wwwroot                ; lives the UI
	%approot%\plugins
	%approot%\data\chains
	%approot%\data\spaces
	%approot%\data\content
	%approot%\internal
	%approot%\lifecycle\next              ; where app packages are deployed (monitored by loader)(***)
	%approot%\lifecycle\previous          ; only accessed by loader, contains history of deployed app packages
   
    ***: Hydrogen.Loader monitors this folder for a "Hydrogen App Package" (a zip file), and as soon as it detects one it:
	  - Stops current node thread
	  - Stops current blazor host thread
	  - deletes %approot\app% folder
	  - unzips app package in lifecycle\next over to \app folder
	  - Reloads the node and blazor host

### Hydrogen App Package

A hydrogen app package is a ZIP file containing the following structure
  - \wwwroot       ; the blazor UI
  - \              ; the node assemblies & config ini

A hydrogen app package is deployed by the loader upon detection in `%approot%\lifecycle\next` folder. The process involves
 - deleting existing \app contents
 - unzipping package over \app
 - copy package to \previous folder
 - deleting package from \next folder

Node is a console application that provides full node features (or light node) AND activates/deactivates the Blazor host for the presentation layer.



Loader -> Node -> Blazor Host (in other thread)


%LAD%/.