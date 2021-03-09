# Framework Architecture

The hydrogen framework is composed of 3 sub-frameworks.

| Framework           | Naming Conventions   | Purpose                                                      |
| ------------------- | -------------------- | ------------------------------------------------------------ |
| Sphere 10 Framework | Sphere10.Framework.* | These are a vast collection of modules for general-purpose usage across all [tiers](3-tier Architecture.md) of an application. Whilst integral to Hydrogen they can also be used stand-alone for non-Hydrogen applications. |
| Helium              | Sphere10.Helium.*    | The Helium framework provides an [enterprise service bus](https://en.wikipedia.org/wiki/Enterprise_service_bus) implementation (modelled after the popular NServiceBus framework) which enables Hydrogen applications to interoperate with one another. Like Sphere 10 Framework, Helium can also be used as a stand-alone framework for non-Hydrogen applications. |
| Hydrogen            | Sphere10.Hydrogen.*  | These modules comprise the P2P blockchain functionality including the node, UI and core engine. The Hydrogen framework depends on both the Sphere 10 and Helium frameworks and thus they are considered part of the "Hydrogen framework". Technically, they are separate frameworks addressing separate domains. |

Each sub-framework within the Hydrogen framework addresses a specific set of [domains](3-tier Architecture.md#Domains) relevant to the applications.  Many of these domains primarily exist to support other domains in the frameworks. Some domains are specifically intended for consumption by framework consumers. Technically, all domains in all the sub-frameworks are available for consumption by developers if they so choose.

1. 

