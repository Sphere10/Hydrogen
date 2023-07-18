using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A stream that can be committed or rolled back.
/// </summary>
public class TransactionalStream<TInnerStream> : StreamDecorator<TInnerStream>, ITransactionalObject where TInnerStream : Stream{
	public event EventHandlerEx<object> Committing { add => _innerTransactionalObject.Committing += value; remove => _innerTransactionalObject.Committing -= value; }
	public event EventHandlerEx<object> Committed { add => _innerTransactionalObject.Committed += value; remove => _innerTransactionalObject.Committed -= value; }
	public event EventHandlerEx<object> RollingBack { add => _innerTransactionalObject.RollingBack += value; remove => _innerTransactionalObject.RollingBack -= value; }
	public event EventHandlerEx<object> RolledBack { add => _innerTransactionalObject.RolledBack += value; remove => _innerTransactionalObject.RolledBack -= value; }

	private readonly ITransactionalObject _innerTransactionalObject;

	public TransactionalStream(TInnerStream innerStream, ITransactionalObject innerTransactionalObject) : base(innerStream) {
		_innerTransactionalObject = innerTransactionalObject;
	}

	public void Commit() => _innerTransactionalObject.Commit();

	public Task CommitAsync() => _innerTransactionalObject.CommitAsync();

	public void Rollback() => _innerTransactionalObject.Rollback();

	public Task RollbackAsync() => _innerTransactionalObject.RollbackAsync();
}

/// <inheritdoc />
public class TransactionalStream : TransactionalStream<ExtendedMemoryStream> {

	public TransactionalStream(string filename, string uncommittedPageFileDir, long pageSize, long maxMemory, bool readOnly = false, bool autoLoad = false) 
		: base(CreateInnerStream(filename, uncommittedPageFileDir, pageSize, maxMemory, readOnly, autoLoad, out var transactionalBuffer), transactionalBuffer) {
	}

	private static ExtendedMemoryStream CreateInnerStream(string filename, string uncommittedPageFileDir, long pageSize, long maxMemory, bool readOnly, bool autoLoad, out TransactionalFileMappedBuffer transactionalBuffer) {
		transactionalBuffer =  new TransactionalFileMappedBuffer(filename, uncommittedPageFileDir, pageSize, maxMemory, readOnly, autoLoad);
		var extendedStream = new ExtendedMemoryStream(transactionalBuffer, true);
		return extendedStream;
	}
}