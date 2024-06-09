// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

[Serializable]
public delegate void EventHandlerEx();


[Serializable]
public delegate void EventHandlerEx<in T>(T arg);


[Serializable]
public delegate void EventHandlerEx<in T1, in T2>(T1 arg1, T2 arg2);


[Serializable]
public delegate void EventHandlerEx<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);


[Serializable]
public delegate void EventHandlerEx<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);


[Serializable]
public delegate void EventHandlerEx<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
