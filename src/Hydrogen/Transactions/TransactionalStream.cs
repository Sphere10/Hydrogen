// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A stream that can be committed or rolled back.
/// </summary>
public class TransactionalStream<TInnerStream> : StreamDecorator<TInnerStream>, ITransactionalObject where TInnerStream : Stream {
	public event EventHandlerEx Committing { add => _innerTransactionalObject.Committing += value; remove => _innerTransactionalObject.Committing -= value; }
	public event EventHandlerEx Committed { add => _innerTransactionalObject.Committed += value; remove => _innerTransactionalObject.Committed -= value; }
	public event EventHandlerEx RollingBack { add => _innerTransactionalObject.RollingBack += value; remove => _innerTransactionalObject.RollingBack -= value; }
	public event EventHandlerEx RolledBack { add => _innerTransactionalObject.RolledBack += value; remove => _innerTransactionalObject.RolledBack -= value; }

	private readonly ITransactionalObject _innerTransactionalObject;

	public TransactionalStream(TInnerStream innerStream, ITransactionalObject innerTransactionalObject) : base(innerStream) {
		_innerTransactionalObject = innerTransactionalObject;
	}

	public void Commit() {
		InnerStream.Flush();
		_innerTransactionalObject.Commit();
	}

	public async Task CommitAsync() {
		await InnerStream.FlushAsync();
		await _innerTransactionalObject.CommitAsync();
	}

	public void Rollback() => _innerTransactionalObject.Rollback();

	public Task RollbackAsync() => _innerTransactionalObject.RollbackAsync();
}

/// <inheritdoc />
public class TransactionalStream : TransactionalStream<ExtendedMemoryStream>, ILoadable {
	public event EventHandlerEx<object> Loading { add => InnerStream.Loading += value; remove => InnerStream.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InnerStream.Loaded += value; remove => InnerStream.Loaded -= value; }

	public TransactionalStream(TransactionalFileDescriptor fileDescriptor, FileAccessMode accessMode = FileAccessMode.Default) 
		: base(CreateInnerStream(fileDescriptor, accessMode, out var transactionalBuffer), transactionalBuffer) {
	}

	private static ExtendedMemoryStream CreateInnerStream(TransactionalFileDescriptor fileDescriptor, FileAccessMode accessMode, out TransactionalFileMappedBuffer transactionalBuffer) {
		transactionalBuffer = new TransactionalFileMappedBuffer(fileDescriptor, accessMode);
		var extendedStream = new ExtendedMemoryStream(transactionalBuffer, true);
		return extendedStream;
	}

	public bool RequiresLoad => InnerStream.RequiresLoad;

	public void Load() => InnerStream.Load();

	public Task LoadAsync() => InnerStream.LoadAsync();
}