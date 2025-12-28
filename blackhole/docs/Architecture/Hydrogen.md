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
| Hydrogen Framework | **Hydrogen.*** | These are a vast collection of modules for general-purpose usage across all [tiers](3-tier-Architecture.md) of an application. Whilst integral to Hydrogen they can also be used stand-alone for non-Hydrogen applications. |
| Helium              | **Sphere10.Helium.***    | The Helium framework provides an [enterprise service bus](https://en.wikipedia.org/wiki/Enterprise_service_bus) implementation (modelled after the popular NServiceBus framework) which enables Hydrogen applications to interoperate with one another. Like Hydrogen Framework, Helium can also be used as a stand-alone framework for non-Hydrogen applications. |
| Hydrogen            | **Hydrogen.DApp.***  | These modules comprise the P2P blockchain functionality including the node, UI and core engine. The Hydrogen framework depends on both the Sphere 10 and Helium frameworks and thus they are considered part of the "Hydrogen framework". Technically, they are separate frameworks addressing separate domains. |

Each sub-framework within the Hydrogen framework addresses a specific set of [domains](3-tier-Architecture.md#Domains) relevant to the applications.  Many of these domains primarily exist to support other domains in the frameworks. Some domains are specifically intended for consumption by framework consumers. Technically, all domains in all the sub-frameworks are available for consumption by developers if they so choose.

### Hydroge Deployment Model

![Hydrogen Deployment Model](resources/Hydrogen-Deployment-Host-AppPackage.png)

### Hydrogen 3-tier Architecture

![Hydrogen 3-tier architecture](../guidelines/resources/Framework-75pct.png)

### Links
1. [Hydrogen Framework Domains](Domains.md#Sphere-10-Framework-Domains)
2. [Helium Domains](Domains.md#Hydrogen-Domains)
3. [Hydrogen Domains](Domains.md#hydrogen-domains)
4. [Runtime Model](Runtime.md)
5. [3-tier architecture](../Guidelines/3-tier-Architecture.md)
6. [Code Styling Guidelines](../Guidelines/Code-Styling.md)

