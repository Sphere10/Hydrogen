using System;
using System.IO;

namespace Sphere10.Framework {

	public interface IStreamProvider : IDisposable {
		Stream OpenReadStream();
		Stream OpenWriteStream();
	}

}