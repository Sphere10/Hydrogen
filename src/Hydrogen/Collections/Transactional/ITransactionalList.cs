using System;

namespace Hydrogen {
	public interface ITransactionalList<T> : IExtendedList<T>, ITransactionalFile, ILoadable, ISynchronizedObject, IDisposable {
	}

}
