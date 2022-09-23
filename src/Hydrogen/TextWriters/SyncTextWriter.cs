using System.Threading.Tasks;

namespace Hydrogen;

public abstract class SyncTextWriter : TextWriterBase {

	protected override Task InternalWriteAsync(string value) => Task.Run(() => InternalWrite(value));
}
