// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Communications;

namespace Hydrogen.DApp.Core.Runtime;

public static class HostProtocolHelper {

	public static Protocol BuildForNode(ICommandHandler<UpgradeMessage> upgradeNodeHandler)
		=> throw new NotImplementedException();
	//=> new ProtocolBuilder()
	//	.Requests
	//		.ForRequest<PingMessage>().RespondWith((_, _) => new PongMessage())
	//	.Commands
	//		.ForCommand<UpgradeMessage>().Execute(upgradeNodeHandler)
	//	.Messages
	//		.UseOnly(BuildMessageSerializer())
	//		.Build();

}
