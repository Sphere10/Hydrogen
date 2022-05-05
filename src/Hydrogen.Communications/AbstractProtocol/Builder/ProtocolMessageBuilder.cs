namespace Hydrogen.Communications {


	public sealed class ProtocolMessageBuilder : FactorySerializerBuilderBase<object, ProtocolMessageBuilder>, IProtocolBuilderMain {
		private readonly ProtocolMode _mode;

		public ProtocolMessageBuilder(IProtocolBuilderMain parent, ProtocolMode mode, IFactorySerializer<object> serializer) 
			: base(serializer) {
			Parent = parent;
			_mode = mode;
		}

		protected IProtocolBuilderMain Parent { get; }

		public ProtocolHandshakeBuilder Handshake => Parent.Handshake;

		public ProtocolRequestBuilder Requests => Parent.Requests;

		public ProtocolResponseBuilder Responses => Parent.Responses;

		public ProtocolCommandBuilder Commands => Parent.Commands;

		public ProtocolMessageBuilder Messages => Parent.Messages;

		public ProtocolBuilder SetMode(int mode) => Parent.SetMode(mode);

		public Protocol Build() => Parent.Build();

		public ProtocolMessageBuilder UseOnly(IFactorySerializer<object> serializer) {
			Guard.ArgumentNotNull(serializer, nameof(serializer));
			base.Serializer = serializer;
			return this;
		}


	}

}
