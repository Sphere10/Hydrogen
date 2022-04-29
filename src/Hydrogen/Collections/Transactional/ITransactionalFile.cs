using System;

namespace Sphere10.Framework {

	public interface ITransactionalFile : ITransactionalObject {

		string Path { get; }

		TransactionalFileMappedBuffer AsBuffer { get; }

	}

}