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
using Microsoft.VisualBasic;

namespace Hydrogen.Communications {

	public class Protocol {

		public Protocol() {
			Handshake = new ProtocolHandshake();
 			Modes = new List<ProtocolMode>();
		}

		public ProtocolHandshake Handshake { get; init; }

		public IList<ProtocolMode> Modes { get; init; }

		public ProtocolMode NewModeDefinition(int number) {
			Guard.ArgumentEquals(number, Modes.Count, nameof(number));
			var modeDef = new ProtocolMode { Number = number };
			Modes.Add(modeDef);
			return modeDef;
		}

		public Result Validate() {
			var result = Result.Default;

			// Validate internal properties set
			if (Handshake is null)
				result.AddError("Handshake is null");

			if (Modes.Count < 1) {
				result.AddError("Missing mode 0 definition");
			}

			// Validate Handshake
			var supportedTypes = Modes[0].MessageSerializer.RegisteredTypes.ToHashSet(); // handshake uses Mode 0 serializers
			if (Handshake.MessageTypes is not null) {
				var missingSerializers = Handshake.MessageTypes.Where(t => !supportedTypes.Contains(t));
				if (missingSerializers.Any())
					foreach (var type in missingSerializers)
						result.AddError($"Missing serializer for handshake type '{type.Name}'");
			}

			// Validate all protocol modes are correct
			foreach (var mode in Modes) {
				var modeValidation = mode.Validate();
				if (modeValidation.Failure)
					result.Merge(modeValidation);
			}

			
			return result;
		}
	}
}
