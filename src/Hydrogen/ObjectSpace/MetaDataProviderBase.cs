// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Threading.Tasks;

namespace Hydrogen;

internal class MetaDataProviderBase : SyncLoadableBase, IObjectContainerMetaDataProvider {
	
	private readonly long _streamOffset;
	private bool _attached;

	public MetaDataProviderBase(ObjectContainer objectContainer, long reservedStreamIndex, long offset) {
		Container = objectContainer;
		Guard.Ensure(objectContainer.RequiresLoad, "Object Container must not be loaded before creating index");
		ReservedStreamIndex = reservedStreamIndex;
		_streamOffset = offset;
		_attached = false;

		objectContainer.Loaded += ObjectContainer_Loaded;
		objectContainer.Clearing += ObjectContainer_Clearing;
		objectContainer.Cleared += ObjectContainer_Cleared;
	}

	public long ReservedStreamIndex { get; }
	public override bool RequiresLoad => Container.RequiresLoad || !_attached;

	protected ObjectContainer Container { get; }

	private Stream _stream;
	protected Stream Stream { 
		get {
			CheckAttached();
			return _stream;
		}
		private set => _stream = value;
	}

	public void Dispose() {
		if (_attached)
			Detach();
	}


	protected override void LoadInternal() {
		if (Container.RequiresLoad)
			throw new InvalidOperationException("Object Container is not loaded");

		if (!_attached)
			Attach();
	}

	protected void CheckAttached() {
		if (!_attached)
			throw new InvalidOperationException("Index is not attached");
	}

	protected void CheckNotAttached() {
		if (_attached)
			throw new InvalidOperationException("Index is already attached");
	}

	private void Attach() {
		CheckNotAttached();
		NotifyLoading();
		Guard.Ensure(Container.StreamContainer.Header.ReservedStreams > 0, "Stream Container has no reserved streams available");
		Guard.Ensure(ReservedStreamIndex < Container.StreamContainer.Header.ReservedStreams, $"Stream at index {ReservedStreamIndex} is not reserved");
		using (Container.StreamContainer.EnterAccessScope()) {
			_attached = true;

			// Open the stream used by the index. No access scope is acquired for the stream
			// and thus all use of the index must take place within an explicit access scope.
			Stream =
				Container
				.StreamContainer
				.Open(ReservedStreamIndex, false, false)
				.AsBounded(_streamOffset, long.MaxValue, allowInnerResize: true);

			// Ensures the stream is at least as long as the offset (the space prior to offset can
			// be used to store header information (i.e. factory info to decide what type of index to load)
			if (Stream.Position < 0)
				Stream.SetLength(0);

		}
		NotifyLoaded();
	}

	private void ObjectContainer_Loaded(object arg) {
		if (!_attached)
			Load();
	}

	private void ObjectContainer_Clearing(object arg) {
		CheckAttached();
		Detach();
	}

	private void ObjectContainer_Cleared(object arg) {
		CheckNotAttached();
		Attach();
	}

	private void Detach() {
		CheckAttached();
		Stream.Dispose();
		Stream = null;
		_attached = false;
	}
	
}
