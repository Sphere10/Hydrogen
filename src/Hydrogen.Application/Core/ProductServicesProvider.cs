//// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//// Author: Herman Schoenfeld
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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


