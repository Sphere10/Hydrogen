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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sphere10.Framework;


namespace Sphere10.Framework.Application {
	
	
	public class ProductServices : IProductServices {

		public ProductServices() {
		}

		private IProductInformationServices InformationServices { get { return ComponentRegistry.Instance.Resolve<IProductInformationServices>(); } }

		private IProductUsageServices UsageServices { get { return ComponentRegistry.Instance.Resolve<IProductUsageServices>(); } }

		private IProductInstancesCounter InstancesCounter { get { return ComponentRegistry.Instance.Resolve<IProductInstancesCounter>(); } }

		public ProductInformation ProductInformation {
			get { return InformationServices.ProductInformation; }
		}

		public ProductUsageInformation ProductUsageInformation {
			get { return UsageServices.ProductUsageInformation; }
		}

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
}
