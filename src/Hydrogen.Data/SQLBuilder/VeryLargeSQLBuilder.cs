// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Data.SQLBuilder;

public class VeryLargeSQLBuilder : SQLBuilderDecorator, IDisposable {
	private readonly IList<LargeCollection<SQLStatement>> _generatedContainers;

	private readonly int _pageSize;

	public static int DefaultScriptPageSize = 5000000;

	public VeryLargeSQLBuilder(ISQLBuilder internaBuilder) : this(internaBuilder, DefaultScriptPageSize) {
	}

	public VeryLargeSQLBuilder(ISQLBuilder internaBuilder, int scriptPageSize)
		: base(internaBuilder) {
		SystemLog.Info($"VeryLargeScriptBuilder created - scriptPageSize = {scriptPageSize}");
		_pageSize = scriptPageSize;
		_generatedContainers = new List<LargeCollection<SQLStatement>>();
		// We clear again, which forces the base implementation to recreate the objectStream
		base.Clear();
	}

	public override ICollection<SQLStatement> CreateContainerForStatements() {
		var container = new LargeCollection<SQLStatement>(
			_pageSize,
			1,
			statement => sizeof(SQLStatementType) + statement.SQL.Length * sizeof(char)
		);
		//objectStream.PageSwapped += (statements, oldPage, newPage) =>
		//    SystemLog.Error("Page Swap: {0} - {1} / {2}", oldPage.PageNumber, newPage.PageNumber, statements.PageCount);
		_generatedContainers.Add(container);
		return container;
	}

	public sealed override ISQLBuilder CreateBuilder() {
		// We do not want to create another VeryLargeScriptBuilder here since sub-builders
		// are used for small token-building purposes.
		return base.CreateBuilder();
	}

	public void Dispose() {
		foreach (var container in _generatedContainers) {
			container.Dispose();
		}
	}
}
