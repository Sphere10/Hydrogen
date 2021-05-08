//-----------------------------------------------------------------------
// <copyright file="EventHandlerEx.cs" company="Sphere 10 Software">
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

using System;

namespace Sphere10.Framework {

	[Serializable]
	public delegate void EventHandlerEx();

	[Serializable]
	public delegate void EventHandlerEx<T>(T arg);

	[Serializable]
	public delegate void EventHandlerEx<T1, T2>(T1 arg1, T2 arg2);

	[Serializable]
	public delegate void EventHandlerEx<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

	[Serializable]
	public delegate void EventHandlerEx<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

	[Serializable]
	public delegate void EventHandlerEx<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
}
