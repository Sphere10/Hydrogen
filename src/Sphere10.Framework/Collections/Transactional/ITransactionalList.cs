using System;

namespace Sphere10.Framework {
	public interface ITransactionalList<T> : IExtendedList<T>, ITransactionalFile, IDisposable {
	}
}
