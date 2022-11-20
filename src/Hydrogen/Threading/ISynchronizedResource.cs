//-----------------------------------------------------------------------
// <copyright file="IReadWriteSafeObject.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

namespace Hydrogen {

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
}
