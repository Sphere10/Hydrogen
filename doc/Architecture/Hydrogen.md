# Hydrogen Framework

Hydrogen is a .NET-based framework for building scalable P2P blockchain applications. It is a broad framework that can be used for building  layer-1 blockchain-based systems as well as layer-2 extensions to those systems. 

The Hydrogen framework offers the following features:

- **Node**: a fully functional node with it's own console-based GUI. The node provides data-persistence for blockchain and consensus databases, networking layer for P2P protocols, key management for wallets, JSON APIs for integration and all the core functionality typically offered by a node.
- **GUI**: a Blazor-based GUI for interacting with the node. This piece includes a rich-set of widgets useful for composing applications.
- **In-protocol upgrades**: applications control how they are upgraded and can be upgraded automatically from the blockchain itself (typically through governance protocol).  Absolutely everything can be upgraded including the node and UI. 
- **Automatic Interoperability**: hydrogen applications can easily interoperate with one another with complex workflows and patterns (publish-subscribe, send-and-forget, sagas) via the Helium framework.
- **Plug-n-Play**:  plugins that can extend both the Node and/or GUI and which can be installed dynamically.
- **Extensible**: in addition to plugins, the the framework offers extension points at various layers of the architecture.
- **Cross-platform**: runs on any OS that supports .NET 5 framework.

The hydrogen framework is composed of 3 sub-frameworks.

| Framework           | Naming Conventions       | Purpose                                                      |
| ------------------- | ------------------------ | ------------------------------------------------------------ |
| Sphere 10 Framework | **Sphere10.Framework.*** | These are a vast collection of modules for general-purpose usage across all [tiers](3-tier-Architecture.md) of an application. Whilst integral to Hydrogen they can also be used stand-alone for non-Hydrogen applications. |
| Helium              | **Sphere10.Helium.***    | The Helium framework provides an [enterprise service bus](https://en.wikipedia.org/wiki/Enterprise_service_bus) implementation (modelled after the popular NServiceBus framework) which enables Hydrogen applications to interoperate with one another. Like Sphere 10 Framework, Helium can also be used as a stand-alone framework for non-Hydrogen applications. |
| Hydrogen            | **Sphere10.Hydrogen.***  | These modules comprise the P2P blockchain functionality including the node, UI and core engine. The Hydrogen framework depends on both the Sphere 10 and Helium frameworks and thus they are considered part of the "Hydrogen framework". Technically, they are separate frameworks addressing separate domains. |

Each sub-framework within the Hydrogen framework addresses a specific set of [domains](3-tier-Architecture.md#Domains) relevant to the applications.  Many of these domains primarily exist to support other domains in the frameworks. Some domains are specifically intended for consumption by framework consumers. Technically, all domains in all the sub-frameworks are available for consumption by developers if they so choose.

1. [Sphere 10 Framework Domains](Domains.md#Sphere-10-Framework-Domains)
2. [Helium Domains](Domains.md#Hydrogen-Domains)
3. [Hydrogen Domains](Domains.md#hydrogen-domains)



## Subsystem composition

![](resources/Hydrogen-Deployment-SubSystems.png)



## Runtime Model

A Hydrogen application consists of the following artefacts:

1. **Hydrogen Host**: the host is an application installed by the user which manages the lifecycle of a Hydrogen Application Package (HAP). The Host is installed once and is expected to never require an upgrade. The host is entry-point for the execution of the HAP and defines the configuration which is passed into the HAP. 
2. **Hydrogen Application Package (HAP)**: A hydrogen application package is a ZIP file which contains all the assemblies for the node and GUI.  HAP's can be deployed, loaded, started, stopped and archived by the host.  During it's own execution, a HAP can deposit a new version of itself in the deployment queue which triggers the host to upgrade the HAP. In this manner, a Hydrogen application can fully upgrade itself automatically.
3. **Consensus Databases**: These are file artefacts which reside in a separate directory to the HAP and Host. Files in these folders comprise the consensus data which is constructed by the HAP in the course of it's operation. These files can be anything, from blockchains to SQL databases, and are available to the HAP. These files are not modified by the Host and only the HAP, thus during an upgrade the HAP must be aware of prior file versions and upgrade them within the HAP. The Hydrogen framework provides components for high-frequency blockchains (consensus streams) and state-databases (object spaces) which in the data folders.

![](resources/Hydrogen-Deployment-Host-AppPackage.png)

### Folder Structure

1. **User Data**: these are file artefacts which reside in a protected directory and provided to the HAP with users permission. This is where wallets containing user keys are stored, and any other sensitive data. 
2. **Content Data**: this is a content database modelled after the git file-control system. Content is stored by it's hash, with first 2 base64 digits being a sub-folder name.
3. **Temp Data**: this is a folder where temporary data is stored for the Hydrogen application. This typically contains transaction-pages and file-swapped data used extensively in the Hydrogen framework.

### HAP Lifecycle

A HAP lifecycle can be in one of the following states.

* Ready: the HAP is sitting in the `in` folder and ready for deployment.
* Deploying: the existing HAP is being archived and the new HAP is being unzipped into the `hap` folder.
* Stopped: the application is deployed, but not running.
* Loading: the host is loading the application. Any upgrades can occur in this phase.
* Started: the node and GUI are running.
* Archiving: the hap is being zipped and archived.

![](resources/HAP-lifecycle.png)

## Code Structuring

The Hydrogen codebase is structured into a [3-tier architecture](../guidelines/3-tier-Architecture.md) and styled according to the these [code styling guidelines](../guidelines/codestyle.md).



![](../guidelines/resources/Framework-75pct.png)

