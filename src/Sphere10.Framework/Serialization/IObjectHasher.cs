using System.Collections.Generic;

namespace Sphere10.Framework {

	public interface IObjectHasher<in TItem> {
		byte[] Hash(TItem @object);
	}

}