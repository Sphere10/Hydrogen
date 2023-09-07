using System;

namespace Hydrogen;

public interface IObjectContainerMetaDataProvider : IDisposable {

	public long ReservedStreamIndex { get; }
}
