// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Object space backed by a transactional file, wiring commit/rollback semantics to the underlying <see cref="TransactionalStream"/>.
/// </summary>
public class FileObjectSpace : ObjectSpace, ITransactionalObject{

	public event EventHandlerEx Committing { add => _fileStream.Committing += value; remove => _fileStream.Committing -= value; }
	public event EventHandlerEx Committed { add => _fileStream.Committed += value; remove => _fileStream.Committed -= value; }
	public event EventHandlerEx RollingBack { add => _fileStream.RollingBack += value; remove => _fileStream.RollingBack -= value; }
	public event EventHandlerEx RolledBack { add => _fileStream.RolledBack += value; remove => _fileStream.RolledBack -= value; }

	private readonly TransactionalStream _fileStream;

	public FileObjectSpace(HydrogenFileDescriptor file, ObjectSpaceDefinition objectSpaceDefinition, SerializerFactory serializerFactory, ComparerFactory comparerFactory, FileAccessMode accessMode = FileAccessMode.Default)
		: base(CreateStreams(file, objectSpaceDefinition.Traits.HasFlag(ObjectSpaceTraits.Merkleized), accessMode, out var fileStream), objectSpaceDefinition, serializerFactory, comparerFactory, accessMode) {
		Guard.ArgumentNotNull(file, nameof(file));
		Guard.ArgumentNotNull(objectSpaceDefinition, nameof(objectSpaceDefinition));
		Guard.ArgumentNotNull(serializerFactory, nameof(serializerFactory));
		Guard.ArgumentNotNull(comparerFactory, nameof(comparerFactory));

		File = file;
		AccessMode = accessMode;

		FlushOnDispose = false; // this flushes on Commiting event, on Dispose we don't want to risk changing the file (Rollback scenario)

		// Create the file stream
		_fileStream = fileStream;
		SubscribeToFileStreamEvents();
		

		if (AccessMode.HasFlag(FileAccessMode.AutoLoad))
			Load();
	}

	public FileAccessMode AccessMode { get; }
	
	public HydrogenFileDescriptor File { get; }

	private static ClusteredStreams CreateStreams(HydrogenFileDescriptor file, bool merkleized, FileAccessMode accessMode, out TransactionalStream fileStream) {
		fileStream = new TransactionalStream(file, accessMode.WithoutAutoLoad());
		var objectSpaceMetaDataStreamCount = merkleized ? 1 : 0;
		var streams = new ClusteredStreams(
			fileStream,
			(int)file.ClusterSize,
			file.ContainerPolicy,
			objectSpaceMetaDataStreamCount,
			file.Endianness,
			false
		);
		streams.OwnsStream = true; // disposes _fileStream
		return streams;
	}

	public void Commit()  {
		using (EnterAccessScope()) {
			// flush all changed (stored in uncommitted pages)
			_fileStream.Commit();
		}
	}

	public Task CommitAsync() => throw new NotSupportedException();
	
	public void Rollback() {
		using (EnterAccessScope()) {
			_fileStream.Rollback();
		}
	}

	public Task RollbackAsync() => throw new NotSupportedException();

	public override void Dispose() {
		UnsubscribeToFileStreamEvents();
		base.Dispose();
	}

	protected virtual void OnCommitting() {
		Flush();
	}

	protected virtual void OnCommitted() {
	}

	protected virtual void OnRollingBack() {
		Unload();
	}

	protected virtual void OnRolledBack() {
		// reload after rollback
		_streams.Initialize();
		Load();
	}

	private void SubscribeToFileStreamEvents() {
		_fileStream.Committing += OnCommitting;
		_fileStream.Committed += OnCommitted;
		_fileStream.RollingBack += OnRollingBack;
		_fileStream.RolledBack += OnRolledBack;
	}

	private void UnsubscribeToFileStreamEvents() {
		if (_fileStream is null)
			return;

		_fileStream.Committing -= OnCommitting;
		_fileStream.Committed -= OnCommitted;
		_fileStream.RollingBack -= OnRollingBack;
		_fileStream.RolledBack -= OnRolledBack;
	}
}
