// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Summary description for ApplicationEvent.
/// </summary>
public class ApplicationChangedEvent : EventArgs {
	public ApplicationChangedEvent(string targetAppName, string sourceApplicationName, DateTime switchedOn, TimeSpan openedFor) {
		TargetProcessName = targetAppName;
		SourceProcessName = sourceApplicationName;
		SwitchedOn = switchedOn;
		SourceOpenedFor = openedFor;
	}

	public string TargetProcessName { get; private set; }
	public string SourceProcessName { get; private set; }
	public DateTime SwitchedOn { get; private set; }
	public TimeSpan SourceOpenedFor { get; private set; }
}
