//-----------------------------------------------------------------------
// <copyright file="IUserInterfaceServices.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Application {


	public interface IUserInterfaceServices  {

		void Exit(bool force = false);

		bool ApplicationExiting { get; set; }

		string Status { get; set; }

		void ExecuteInUIFriendlyContext(Action function, bool executeAsync = false);

		void ShowNagScreen(bool modal = false, string nagMessage = null);

		object PrimaryUIController { get; }

	}
}
