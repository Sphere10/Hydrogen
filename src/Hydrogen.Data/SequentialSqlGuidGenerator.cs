// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Hydrogen.Data;

public class SequentialSqlGuidGenerator : ISequentialGuidGenerator {
	private Stack<SqlGuid> _guidStack;

	public SequentialSqlGuidGenerator(long capacity, bool regenerateWhenEmpty = true) {
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
		return (Guid)_guidStack.Pop();
	}

	private void GenerateGuids() {
		var list = new List<SqlGuid>();
		for (var i = 0; i < Capacity; i++)
			list.Add(new SqlGuid(Guid.NewGuid()));

		list.Sort();
		list.Reverse();
		_guidStack = new Stack<SqlGuid>(list);

	}

}
