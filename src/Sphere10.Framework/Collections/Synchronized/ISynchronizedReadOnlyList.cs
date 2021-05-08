using System.Collections.Generic;

namespace Sphere10.Framework {
	public interface ISynchronizedReadOnlyList<T> : IReadOnlyList<T>, ISynchronizedObject {
	}
}
