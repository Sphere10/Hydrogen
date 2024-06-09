// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public interface ISynchronizedResource : ISynchronizedResource<Scope, Scope> {
}


public interface ISynchronizedResource<TReadScope, TWriteScope> : ISynchronizedObject<TReadScope, TWriteScope>
	where TReadScope : IScope
	where TWriteScope : IScope {

	event EventHandlerEx<object> InitializingRead;
	event EventHandlerEx<object> InitializedRead;
	event EventHandlerEx<object> FinalizingRead;
	event EventHandlerEx<object> FinalizedRead;
	event EventHandlerEx<object> InitializingWrite;
	event EventHandlerEx<object> InitializedWrite;
	event EventHandlerEx<object> FinalizingWrite;
	event EventHandlerEx<object> FinalizedWrite;

}
