// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Communications {

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

