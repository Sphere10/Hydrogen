// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public class PreGenSequentialGuidGenerator : ISequentialGuidGenerator {
	private Stack<Guid> _guidStack;

	public PreGenSequentialGuidGenerator(long capacity, bool regenerateWhenEmpty = true) {
		Capacity = capacity;
		RegenerateWhenEmpty = regenerateWhenEmpty;
		GenerateGuids();
	}

	public long Capacity { get; set; }

	public bool RegenerateWhenEmpty { get; set; }

	public int Count {
		get { return _guidStack.Count; }
	}

	public Guid NextSequentialGuid() {
		if (_guidStack.Count == 0) {
			if (RegenerateWhenEmpty)
				GenerateGuids();
			else throw new SoftwareException("No more guids are available");
		}
		return _guidStack.Pop();
	}

	protected void GenerateGuids() {
		var list = new List<Guid>();
		for (var i = 0; i < Capacity; i++)
			list.Add(Guid.NewGuid());

		list.Sort();
		list.Reverse();
		_guidStack = new Stack<Guid>(list);

	}

}
