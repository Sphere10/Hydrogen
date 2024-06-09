// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;

namespace Hydrogen.Communications;

public class ProtocolBuilder {

	private readonly Protocol _protocol;

	public ProtocolBuilder() : this(SerializerFactory.Default) { }

	public ProtocolBuilder(SerializerFactory baseFactory) {
		Guard.ArgumentNotNull(baseFactory, nameof(baseFactory));
		_protocol = new Protocol();
		ActiveMode = 0;
	}

	public int ActiveMode { get; private set; }

	public SerializerFactory BaseFactory { get; }

	public ProtocolBuilder SetMode(int mode) {
		Guard.Argument(mode != 0, nameof(mode), "Handshake (mode 0) cannot be edited directly, use Handshake property.");
		Guard.ArgumentInRange(mode, 1, _protocol.Modes.Length, nameof(mode));
		if (mode == _protocol.Modes.Length) 
			_protocol.AddMode();
		ActiveMode = mode;
		return this;
	}

	public ProtocolBuilder ConfigureHandshake(Action<ProtocolHandshakeBuilder> handshakeBuild) {
		var handshakeBuilder = new ProtocolHandshakeBuilder(this);
		handshakeBuild(handshakeBuilder);
		_protocol.Handshake = handshakeBuilder.Build();
		return this;
	}

	public ProtocolBuilder ConfigureRequest<TRequest>(Action<ProtocolRequestBuilder<TRequest>> requestBuild) {
		var rrb = new ProtocolRequestBuilder<TRequest>();
		requestBuild(rrb);
		var rr = rrb.Build();
		return AddRequestResponse(rr.Item1, rr.Item2);
	}

	public ProtocolBuilder AddRequestResponse<TRequest, TResponse>(Func<TResponse> requestHandler, Action responseHandler)
		=> AddRequestResponse<TRequest, TResponse>((_, _) => requestHandler(), (_, _, _) => responseHandler());

	public ProtocolBuilder AddRequestResponse<TRequest, TResponse>(Func<TRequest, TResponse> requestHandler, Action<TRequest, TResponse> responseHandler)
		=> AddRequestResponse<TRequest, TResponse>((_, request) => requestHandler(request), (_, request, response) => responseHandler(request, response));

	public ProtocolBuilder AddRequestResponse<TRequest, TResponse>(Func<ProtocolOrchestrator, TRequest, TResponse> requestHandler, Action<ProtocolOrchestrator, TRequest, TResponse> responseHandler )
		=> AddRequestResponse(
			new ActionRequestHandler<TRequest, TResponse>(requestHandler), 
			new ActionResponseHandler<TRequest, TResponse>(responseHandler)
		);

	public ProtocolBuilder AddRequestResponse(IRequestHandler requestHandler, IResponseHandler responseHandler) {
		Guard.ArgumentNotNull(requestHandler, nameof(requestHandler));
		Guard.ArgumentNotNull(responseHandler, nameof(responseHandler));
		Guard.Ensure(requestHandler.RequestType == responseHandler.RequestType, "Request type mismatch on handlers");
		Guard.Ensure(requestHandler.ResponseType == responseHandler.ResponseType, "Response type mismatch on handlers");
		_protocol.Modes[ActiveMode].RequestHandlers.Add(requestHandler.RequestType, requestHandler);
		_protocol.Modes[ActiveMode].ResponseHandlers.Add(responseHandler.RequestType, responseHandler.ResponseType, responseHandler);
		return this;
	}

	public ProtocolBuilder ConfigureCommand<TMessage>(Action<ProtocolCommandBuilder<TMessage>> commandBuild) {
		var cb = new ProtocolCommandBuilder<TMessage>();
		commandBuild(cb);
		var c = cb.Build();
		return AddCommand(c);
	}

	public ProtocolBuilder AddCommand<TMessage>(Action commandHandler)
		=> AddCommand<TMessage>(_ => commandHandler());

	public ProtocolBuilder AddCommand<TMessage>(Action<TMessage> commandHandler) 
		=> AddCommand<TMessage>((_, m) => commandHandler(m));

	public ProtocolBuilder AddCommand<TMessage>(Action<ProtocolOrchestrator, TMessage> commandHandler) 
		=> AddCommand(new ActionCommandHandler<TMessage>(commandHandler));

	public ProtocolBuilder AddCommand(ICommandHandler commandHandler) {
		Guard.ArgumentNotNull(commandHandler, nameof(commandHandler));
		_protocol.Modes[ActiveMode].CommandHandlers.Add(commandHandler.MessageType, commandHandler);
		return this;
	}

	public ProtocolBuilder ConfigureSerialization(Action<SerializerFactory> serializerFactoryConfig) {
		serializerFactoryConfig(BaseFactory);
		return this;
	}

	public ProtocolBuilder AutoBuildSerializers() {
		foreach(var type in _protocol.Modes.SelectMany(x => x.GetAllUsedMessageTypes())) 
			if (!BaseFactory.ContainsSerializer(type))
				BaseFactory.RegisterAutoBuild(type);
		return this;
	}

	public Protocol Build() {
		// Convention: Handshake uses Mode 0 serializers (App)
		var result = _protocol.Validate();
		if (result.IsFailure)
			throw new ProtocolBuilderException(result.ErrorMessages.ToDelimittedString(Environment.NewLine));
		return _protocol;
	}

}
