// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications;

public sealed class ProtocolMessageBuilder : ProtocolSerializerBuilderBase<object, ProtocolMessageBuilder>, IProtocolBuilderMain {
	private readonly ProtocolMode _mode;

	public ProtocolMessageBuilder(IProtocolBuilderMain parent, ProtocolMode mode, PolymorphicSerializer<object> serializer)
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

	public ProtocolMessageBuilder UseOnly(PolymorphicSerializer<object> serializer) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		base.Serializer = serializer;
		return this;
	}


}
