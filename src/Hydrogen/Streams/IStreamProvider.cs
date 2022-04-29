using System;
using System.IO;

namespace Hydrogen {

	public interface IStreamProvider : IDisposable {
		Stream OpenReadStream();
		Stream OpenWriteStream();
	}

}