// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace Tools;

public static class Debugger {

	public static bool BreakConditionA { get; set; } = false;
	public static bool BreakConditionB { get; set; } = false;
	public static bool BreakConditionC { get; set; } = false;
	public static bool BreakConditionD { get; set; } = false;
	public static bool BreakConditionE { get; set; } = false;
	public static int CounterA { get; set; } = 0;
	public static int CounterB { get; set; } = 0;
	public static int CounterC { get; set; } = 0;
	public static int CounterD { get; set; } = 0;
	public static int CounterE { get; set; } = 0;
	public static object ObjectA { get; set; } = null;
	public static object ObjectB { get; set; } = null;
	public static object ObjectC { get; set; } = null;
	public static object ObjectD { get; set; } = null;
	public static object ObjectE { get; set; } = null;
	public static List<string> Messages { get; } = new(4096);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddMessage(string message) => Messages.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");

}
