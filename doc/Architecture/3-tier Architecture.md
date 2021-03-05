# 3-tier Architecture

A  3-tier architecture decomposes a system into 3 primary tiers.

1. **Presentation**: responsible for all aspects of the User Interface (UI). Includes graphical, console and service application executables. Contains all presentation-oriented modules that may depend on processing-tier modules for acquiring their data models defined in the Data Objects tier.

2. **Processing**: responsible for computational and algorithmic modules. Includes application logic, business logic, consensus rules, security, cryptography.  This tier contains computational-oriented projects which do not depend on user interaction, but may on data tier.

3. **Data**: responsible for storage-oriented modules. Includes database drivers, blockchain databases, relational databases, file-based storage formats.


### Ancillary-tiers

4. **Communications**: responsible for all communication modules in the application. These modules include TCP/IP libraries, JSON API clients and services, Web Sockets libraries for the application. This tier is contained within the Data-tier.

5. **Data Objects**: a collection of modules that define data objects used throughout the 3 primary tiers. These include light-weight data objects, POCO, file format definitions, database entities, blockchain objects, data-transfer objects (DTOs).

6. **System**: general-purpose code that with no dependencies that can be used at any tier.

![3-Tier Architecture](resources/3-Tier-Architecture-75pct.png)


## Domains

A domain is an aspect of the application which spans across some (or all) the tiers. It is essentially a vertical slice through the architecture which models models a logically related set of abstractions. For example, in a "School" model the domains may include Teachers, Students, Admin, Classes, Subjects, Exams, etc.  Each of those domains has modules in the presentation, processing and data tiers respectively (as well as ancillary tiers). However, some domains may only have modules in one or two tiers.



![Domains](resources/Domains-75pct.png)



## Modules

Whereas a domain is a vertical-slice across multiple-tiers, a  module is a horizontal slice within single tier. A module is synonymous with a "code-library" and a ".NET project". A module can also consist of the executable project (GUI, console, plugin), resources packages for graphics, sounds, etc.   

![Modules](resources/Modules-75pct.png)

#### Naming Convention

The naming convention for modules is: `Company.Product.Tier.Domain`.

**Examples**

```
McDonalds.PointOfSale.Presentation.WinFormsApp
McDonalds.PointOfSale.Presentation.iOSApp
McDonalds.PointOfSale.Processing.Billing
McDonalds.PointOfSale.Processing.Rostering
McDonalds.PointOfSale.DataObjects.Billing  
McDonalds.PointOfSale.DataObjects.Rostering
McDonalds.PointOfSale.Data.Rostering

Sphere10.Hydrogen.Core     ; "Core" can be used as a "catch all" for the core tiers.
```



## Framework

A framework is a collection of modules structured and organized into a 3-tier architecture. Frameworks offer domains of functionality which are used to construct applications.

![Framework](resources/Framework-75pct.png)

