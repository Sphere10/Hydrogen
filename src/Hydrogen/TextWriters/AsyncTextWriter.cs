namespace Hydrogen;

public abstract class AsyncTextWriter : TextWriterBase {

	protected override void InternalWrite(string value) => InternalWriteAsync(value).WaitSafe();
	
}
