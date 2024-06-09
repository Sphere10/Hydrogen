// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Communications;

public class Protocol {
	private ProtocolMode[] _modes;
	public Protocol() {
		Handshake = new ProtocolHandshake();
		_modes = Array.Empty<ProtocolMode>();
		Factory = new SerializerFactory(SerializerFactory.Default);
		AddMode();
	}

	public ProtocolHandshake Handshake { get; set; }

	public ProtocolMode[] Modes => _modes;

	public SerializerFactory Factory { get; set; }

	public ProtocolMode AddMode() {
		var modeDef = new ProtocolMode { Number = _modes.Length };
		Array.Resize(ref _modes, _modes.Length + 1);
		_modes[^1] = modeDef;
		return modeDef;
	}

	public Result Validate() {
		var result = Result.Default;

		// Validate internal properties set
		if (Handshake is null)
			result.AddError("Handshake is null");

		if (Modes.Length < 1) {
			result.AddError("Missing mode 0 definition");
		}

		
		// Validate all protocol modes are correct
		foreach (var mode in Modes) {
			var modeValidation = mode.Validate();
			if (modeValidation.IsFailure)
				result.Merge(modeValidation);
		}

		// Check missing serializers
		var missingSerializers = 
			Handshake
			.MessageTypes
			.Union(Modes.SelectMany(x => x.GetAllUsedMessageTypes()))
			.Where(x => !Factory.ContainsSerializer(x))
			.ToArray();

			if (missingSerializers.Any())
				foreach (var type in missingSerializers)
					result.AddError($"Missing serializer for type '{type.Name}'");
			
			return result;
		}

}

