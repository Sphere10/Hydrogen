using Sphere10.Framework;
using Sphere10.Framework.Communications;
using System.Collections.Generic;

namespace Sphere10.Hydrogen.Core.Runtime {

	public static class HostProtocolHelper {

		public static Protocol BuildForNode<TChannel>(ICommandHandler<TChannel, UpgradeMessage> upgradeNodeHandler) where TChannel : ProtocolChannel
			=> new ProtocolBuilder<TChannel>()
				.Requests
					.ForRequest<PingMessage>().RespondWith((_, _) => new PongMessage())
				.Commands
					.ForCommand<UpgradeMessage>().Execute(upgradeNodeHandler)
				.MessageDefinitions
					.Use(BuildMessageSerializer())
				.Build();

		public static IFactorySerializer<object> BuildMessageSerializer()
			=> new FactorySerializerBuilder<object>()
				.For<PingMessage>(HostProtocolMessageType.Ping).SerializeWith(new BinaryFormattedSerializer<PingMessage>())
				.For<PongMessage>(HostProtocolMessageType.Pong).SerializeWith(new BinaryFormattedSerializer<PongMessage>())
				.For<RollbackMessage>(HostProtocolMessageType.Rollback).SerializeWith(new BinaryFormattedSerializer<RollbackMessage>())
				.For<ShutdownMessage>(HostProtocolMessageType.Shutdown).SerializeWith(new BinaryFormattedSerializer<ShutdownMessage>())
				.For<UpgradeMessage>(HostProtocolMessageType.Upgrade).SerializeWith(new BinaryFormattedSerializer<UpgradeMessage>())
				.Build();
	}
}
