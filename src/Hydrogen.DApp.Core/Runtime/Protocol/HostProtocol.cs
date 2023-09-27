// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Communications;

namespace Hydrogen.DApp.Core.Runtime;

public static class HostProtocolHelper {

	public static Protocol BuildForNode(ICommandHandler<UpgradeMessage> upgradeNodeHandler)
		=> new ProtocolBuilder()
			.Requests
				.ForRequest<PingMessage>().RespondWith((_, _) => new PongMessage())
			.Commands
				.ForCommand<UpgradeMessage>().Execute(upgradeNodeHandler)
			.Messages
				.UseOnly(BuildMessageSerializer())
				.Build();

	public static FactorySerializer<object> BuildMessageSerializer()
		=> new ProtocolSerializerBuilder<object>()
			.For<PingMessage>(HostProtocolMessageType.Ping).SerializeWith(new BinaryFormattedSerializer<PingMessage>())
			.For<PongMessage>(HostProtocolMessageType.Pong).SerializeWith(new BinaryFormattedSerializer<PongMessage>())
			.For<RollbackMessage>(HostProtocolMessageType.Rollback).SerializeWith(new BinaryFormattedSerializer<RollbackMessage>())
			.For<ShutdownMessage>(HostProtocolMessageType.Shutdown).SerializeWith(new BinaryFormattedSerializer<ShutdownMessage>())
			.For<UpgradeMessage>(HostProtocolMessageType.Upgrade).SerializeWith(new BinaryFormattedSerializer<UpgradeMessage>())
			.Build();
}
