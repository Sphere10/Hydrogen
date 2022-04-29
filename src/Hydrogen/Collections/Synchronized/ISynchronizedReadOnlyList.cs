using System.Collections.Generic;

namespace Hydrogen {
	public interface ISynchronizedReadOnlyList<T> : IReadOnlyList<T>, ISynchronizedObject {
	}
}
