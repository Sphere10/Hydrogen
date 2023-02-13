////-----------------------------------------------------------------------
//// <copyright file="ProductProvider.cs" company="Sphere 10 Software">
////
//// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// <author>Herman Schoenfeld</author>
//// <date>2018</date>
//// </copyright>
////-----------------------------------------------------------------------

//namespace Hydrogen.Application;


//public class ProductServicesProvider : IProductInformationProvider, IProductUsageServices, IProductInstancesCounter {

//	public ProductServicesProvider(IProductInformationProvider productInformationProvider, IProductUsageServices productUsageServices, IProductInstancesCounter productInstancesCounter) {
//		InformationProvider = productInformationProvider;
//		UsageServices = productUsageServices;
//		InstancesCounter = productInstancesCounter;
//	}

//	private IProductInformationProvider InformationProvider { get; }

//	private IProductUsageServices UsageServices { get; }

//	private IProductInstancesCounter InstancesCounter { get; }

//	public ProductInformation ProductInformation => InformationProvider.ProductInformation;

//	public ProductUsageInformation ProductUsageInformation => UsageServices.ProductUsageInformation;

//	public int CountNumberOfRunningInstances() {
//		return InstancesCounter.CountNumberOfRunningInstances();
//	}

//	public void IncrementUsageByOne() {
//		UsageServices.IncrementUsageByOne();
//	}

//}

