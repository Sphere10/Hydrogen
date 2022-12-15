//-----------------------------------------------------------------------
// <copyright file="ProductServices.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

namespace Hydrogen.Application;


public class ProductServices : IProductServices {

	public ProductServices(IProductInformationServices productInformationServices, IProductUsageServices productUsageServices, IProductInstancesCounter productInstancesCounter) {
		InformationServices = productInformationServices;
		UsageServices = productUsageServices;
		InstancesCounter = productInstancesCounter;
	}

	private IProductInformationServices InformationServices { get; }

	private IProductUsageServices UsageServices { get; }

	private IProductInstancesCounter InstancesCounter { get; }

	public ProductInformation ProductInformation => InformationServices.ProductInformation;

	public ProductUsageInformation ProductUsageInformation => UsageServices.ProductUsageInformation;

	public int CountNumberOfRunningInstances() {
		return InstancesCounter.CountNumberOfRunningInstances();
	}

	public void IncrementUsageByOne() {
		UsageServices.IncrementUsageByOne();
	}

	public string ProcessString(string str) {
		str = ProductInformation.ProcessTokensInString(str);
		str = ProductUsageInformation.ProcessTokensInString(str);
		return str;
	}
}

