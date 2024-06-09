// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class ActionJob : BaseJob {
	private readonly Action _action;

	public ActionJob(Action action, string name = null) {
		_action = action;
		Name = name ?? $"Action Job {Guid.NewGuid().ToStrictAlphaString()}";
	}

	public override void Execute() {
		_action();
	}

	public override JobSerializableSurrogate ToSerializableSurrogate() {
		throw new NotSupportedException("Cannot serialize action jobs");
	}

	public override void FromSerializableSurrogate(JobSerializableSurrogate jobSurrogate) {
		throw new NotSupportedException("Cannot deserialize action jobs");
	}
}
