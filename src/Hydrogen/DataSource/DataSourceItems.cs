using System.Collections.Generic;

namespace Hydrogen;

public struct DataSourceItems<TItem> {

	public IEnumerable<TItem> Items { get; init; }

	public int Page { get; init; }
		
	public int TotalCount { get; init; }
}
