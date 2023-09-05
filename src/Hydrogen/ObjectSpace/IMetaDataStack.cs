using System;

namespace Hydrogen.ObjectSpace.MetaData;

public interface IMetaDataStack : IDisposable {
	IStack<long> Stack { get; }
	
}
