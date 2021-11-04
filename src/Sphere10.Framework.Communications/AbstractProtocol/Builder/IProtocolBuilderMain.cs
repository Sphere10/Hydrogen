namespace Sphere10.Framework.Communications {

	public interface IProtocolBuilderMain {

		ProtocolHandshakeBuilder Handshake { get; }

		ProtocolRequestBuilder Requests { get; }

		ProtocolResponseBuilder Responses { get; }

		ProtocolCommandBuilder Commands { get; }

		ProtocolMessageBuilder Messages { get; }

		ProtocolBuilder SetMode(int mode);

		Protocol Build();
	}
}
