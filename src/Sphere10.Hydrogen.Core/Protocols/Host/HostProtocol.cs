using Sphere10.Framework;
using Sphere10.Framework.Communications;
using System.Collections.Generic;

namespace Sphere10.Hydrogen.Core.Protocols.Host {


	public static class HostProtocolHelper {



		public static Protocol BuildForNode<TChannel>(ICommandHandler<TChannel, Upgrade> upgradeNodeHandler) where TChannel : ProtocolChannel
			=> new ProtocolBuilder<TChannel>()
				.Requests
					.ForRequest<Ping>().RespondWith((_, _) => new Pong())
				.Commands
					.ForCommand<Upgrade>().Execute(upgradeNodeHandler)
				.Messages
					.Use(BuildMessageSerializer())
				.Build();



		public static IFactorySerializer<object> BuildMessageSerializer()
			=> new FactorySerializerBuilder<object>()
				.For<Ping>(HostProtocolMessageType.Ping).SerializeWith(new BinaryFormattedSerializer<Ping>())
				.For<Pong>(HostProtocolMessageType.Pong).SerializeWith(new BinaryFormattedSerializer<Pong>())
				.For<Rollback>(HostProtocolMessageType.Rollback).SerializeWith(new BinaryFormattedSerializer<Rollback>())
				.For<Shutdown>(HostProtocolMessageType.Shutdown).SerializeWith(new BinaryFormattedSerializer<Shutdown>())
				.For<Upgrade>(HostProtocolMessageType.Upgrade).SerializeWith(new BinaryFormattedSerializer<Upgrade>())
				.Build();


	}
}
