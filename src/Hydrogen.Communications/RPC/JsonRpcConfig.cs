// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications.RPC;

public class JsonRpcConfig {
	public static JsonRpcConfig Default = new JsonRpcConfig();


	public enum ConnectionModeEnum {
		Persistant,
		Pulsed
	};


	//True: Indicate client/server are in persistant mode. The connection is keept alive and the ClientHandler will loop on the EndPoint to process all calls. (Stratum like server mode)
	//False: Indicate client/server are in pulse mode. The onnection is closed at every RPC call. (Default JsonRpc server mode)
	public ConnectionModeEnum ConnectionMode { get; set; } = ConnectionModeEnum.Pulsed;

	//Ignore return values completly, if the RPC has no return type. (ex. 'miner.notify' in Stratum like server mode)
	public bool IgnoreEmptyReturnValue { get; set; } = false;

	//In persistant mode : Max time, in millisec, RPC caller should wait until this is considered a failed call.
	public int MaxTimeWaitingForResult = 30000;

	public ILogger Logger { get; set; } = null;
}
