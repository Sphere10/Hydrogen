// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications.RPC;

//Declare a communication endpoint for simmple messaging
public interface IEndPoint {
	public string GetDescription();

	public ulong GetUID();

	public IEndPoint WaitForMessage();

	public EndpointMessage ReadMessage();

	public void WriteMessage(EndpointMessage message);

	public bool IsOpened();

	public void Start();

	public void Stop();
}
