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

public class ProtocolMode {

	public ProtocolMode() {
		Number = 0;
		CommandHandlers = new Dictionary<Type, ICommandHandler>();
		RequestHandlers = new Dictionary<Type, IRequestHandler>();
		ResponseHandlers = new MultiKeyDictionary<Type, Type, IResponseHandler>();
		MessageGenerators = new Dictionary<Type, IMessageGenerator>();
	}

	public int Number { get; init; }

	public IDictionary<Type, IRequestHandler> RequestHandlers { get; init; }

	public MultiKeyDictionary<Type, Type, IResponseHandler> ResponseHandlers { get; init; }

	public IDictionary<Type, ICommandHandler> CommandHandlers { get; init; }

	public IDictionary<Type, IMessageGenerator> MessageGenerators { get; init; }

	public Result Validate() {
		var result = Result.Default;

		if (MessageGenerators is null)
			result.AddError($"Mode {Number}: MessageGenerators is null");

		if (CommandHandlers is null)
			result.AddError($"Mode {Number}: CommandHandlers is null");

		if (RequestHandlers is null)
			result.AddError($"Mode {Number}: RequestHandlers is null");

		if (ResponseHandlers is null)
			result.AddError($"Mode {Number}: ResponseHandlers is null");

		// Validate message generators generate correct type
		if (MessageGenerators is not null) {
			foreach (var (key, value) in MessageGenerators) {
				if (value.GetType().IsConstructedGenericTypeOf(typeof(IMessageGenerator<>))) {
					var handlerCreatedType = value.GetType().GetGenericArguments()[0];
					if (handlerCreatedType != key)
						result.AddError($"Message generator for type '{key.Name}' activates an object of type '{handlerCreatedType.Namespace}'");
				}
			}
		}

		return result;
	}

	internal IEnumerable<Type> GetAllUsedMessageTypes() 
		=> RequestHandlers.Keys.Union(ResponseHandlers.Keys.SelectMany(x => new[] {x.Item1, x.Item2 })).Union(CommandHandlers.Keys).Union(MessageGenerators.Keys);

}
