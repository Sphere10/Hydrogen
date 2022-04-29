using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework.Communications {

	public abstract class ProtocolBuilderMain : IProtocolBuilderMain {

		public ProtocolBuilderMain(IProtocolBuilderMain parent) {
			Parent = parent;
		}

		protected IProtocolBuilderMain Parent { get; }

		public virtual ProtocolHandshakeBuilder Handshake => Parent.Handshake;

		public virtual ProtocolRequestBuilder Requests => Parent.Requests;

		public virtual ProtocolResponseBuilder Responses => Parent.Responses;

		public virtual ProtocolCommandBuilder Commands => Parent.Commands;

		public virtual ProtocolMessageBuilder Messages => Parent.Messages;

		public virtual ProtocolBuilder SetMode(int mode) => Parent.SetMode(mode);

		public virtual Protocol Build() => Parent.Build();
	}

}

