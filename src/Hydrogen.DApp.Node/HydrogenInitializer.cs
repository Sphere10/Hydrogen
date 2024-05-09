// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Application;

namespace Hydrogen.DApp.Node;

public interface IHydrogenA {
}


public class HydrogenInitializer : ApplicationInitializerBase {

	public override void Initialize() {
		SystemLog.RegisterLogger(new TimestampLogger(new ConsoleLogger()));

		//NOTE: Until HydrogenInitializer gets to properly reference CryptoEx module, we init it here.
		//Hydrogen.CryptoEx.HydrogenFrameworkIntegration.Initialize();
		//SystemLog.RegisterLogger(new TimestampLogger(new DebugLogger()));


		//TODO: fetch server's init values from some global config module
		//RpcServer.Start(true, 27000, 32);
	}
}
