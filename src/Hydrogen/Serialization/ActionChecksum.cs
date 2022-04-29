using System;

namespace Sphere10.Framework;

public class ActionChecksum<TItem> : IItemChecksum<TItem> {
	private readonly Func<TItem, int> _actionChecksum;

	public ActionChecksum(Func<TItem, int> actionChecksum) {
		_actionChecksum = actionChecksum;
	}

	public int Calculate(TItem item) => _actionChecksum(item);

}