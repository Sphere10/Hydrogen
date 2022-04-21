using System.Collections.Generic;

namespace Sphere10.Framework;

public struct DataSourceItems<TItem> {

	public IEnumerable<TItem> Items { get; init; }

	public int Page { get; init; }
		
	public int TotalCount { get; init; }
}
