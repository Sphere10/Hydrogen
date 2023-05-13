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

	public class ProtocolBuilder : IProtocolBuilderMain {

		private readonly Protocol _protocol;

		public ProtocolBuilder()   {
			_protocol = new Protocol();
			Modes = new List<ProtocolModeBuilder>();
			Modes.Add(new ProtocolModeBuilder(0, this, _protocol.NewModeDefinition(0))); // Handshake mode is always 0
			ActiveMode = 0;
			Handshake = new ProtocolHandshakeBuilder(this, _protocol.Handshake);
		}

		public int ActiveMode { get; private set;  }

		public IList<ProtocolModeBuilder> Modes { get; }

		public ProtocolHandshakeBuilder Handshake { get; } // owned by this
		
		public ProtocolRequestBuilder Requests => Modes[ActiveMode].Requests;  // routes to active mode builder

		public ProtocolResponseBuilder Responses => Modes[ActiveMode].Responses; // routes to active mode builder

		public ProtocolCommandBuilder Commands => Modes[ActiveMode].Commands; // routes to active mode builder

		public ProtocolMessageBuilder Messages => Modes[ActiveMode].Messages; // routes to active mode builder

		public ProtocolBuilder SetMode(int mode) {
			Guard.Argument(mode != 0, nameof(mode), "Handshake (mode 0) cannot be edited directly, use Handshake property.");
			Guard.ArgumentInRange(mode, 1, Modes.Count, nameof(mode));
			if (mode == Modes.Count) {
				Modes.Add(new ProtocolModeBuilder(mode, this, _protocol.NewModeDefinition(mode)));
			}
			ActiveMode = mode;
			return this;
		}

        public Protocol Build() {
			// Convention: Handshake uses Mode 0 serializers (App)
			var result = _protocol.Validate();
            if (result.Failure)
                throw new ProtocolBuilderException(result.ErrorMessages.ToDelimittedString(Environment.NewLine));
            return _protocol;
        }
       
    }
}
