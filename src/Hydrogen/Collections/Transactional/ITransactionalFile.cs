using System;

namespace Hydrogen {

	public interface ITransactionalFile : ITransactionalObject {

		string Path { get; }

		TransactionalFileMappedBuffer AsBuffer { get; }

	}

}