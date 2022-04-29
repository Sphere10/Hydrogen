//-----------------------------------------------------------------------
// <copyright file="IAutoRunServices.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Application {

	public interface IAutoRunServices {

		bool DoesAutoRun(AutoRunType type, string applicationName, string executable);

		void SetAutoRun(AutoRunType type, string applicationName, string executable);

		void RemoveAutoRun(AutoRunType type, string applicationName, string executable);

	}
}