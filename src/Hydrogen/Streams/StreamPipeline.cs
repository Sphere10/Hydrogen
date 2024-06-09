// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Hydrogen;

public class StreamPipeline : IDisposable {
	private readonly ManualResetEvent _lastStagedFinished;
	private readonly Action<Stream, Stream>[] _filters;
	private List<BlockingStream> _blockingStreams;

	public StreamPipeline(params Action<Stream, Stream>[] filters) {
		if (filters == null) throw new ArgumentNullException(nameof(filters));
		if (filters.Length == 0 || Array.IndexOf(filters, null) >= 0)
			throw new ArgumentException(nameof(filters));

		_filters = filters;

		_blockingStreams = new List<BlockingStream>(_filters.Length - 1);
		for (var i = 0; i < filters.Length - 1; i++) {
			_blockingStreams.Add(new BlockingStream());
		}
		_lastStagedFinished = new ManualResetEvent(false);
	}

	public void Run(Stream input, Stream output) {
		if (_blockingStreams == null)
			throw new ObjectDisposedException(GetType().Name);
		if (input == null) throw new ArgumentNullException(nameof(input));
		if (!input.CanRead) throw new ArgumentException(nameof(input));
		if (output == null) throw new ArgumentNullException(nameof(output));
		if (!output.CanWrite) throw new ArgumentException(nameof(output));

		var errors = new SynchronizedList<Exception>();
		ThreadStart lastStage = null;
		for (var i = 0; i < _filters.Length; i++) {
			var stageInput = i == 0 ? input : _blockingStreams[i - 1];
			var stageOutput =
				i == _filters.Length - 1 ? output : _blockingStreams[i];
			var filter = _filters[i];

			void Stage() {
				try {
					filter(stageInput, stageOutput);
					if (stageOutput is BlockingStream blockingStream)
						blockingStream.SetEndOfStream();
				} catch (Exception error) {
					errors.Add(error);
					if (stageOutput is BlockingStream blockingStream)
						blockingStream.SetEndOfStreamDueToFailure();
				}
			}

			if (i < _filters.Length - 1) {
				var t = new Thread(Stage) { IsBackground = true };
				t.Start();
			} else lastStage = Stage;
		}
		if (!errors.Any()) {
			try {
				lastStage();
			} catch (Exception error) {
				errors.Add(error);
			}
		}
		if (errors.Any()) {
			throw new AggregateException(errors);
		}
	}

	public void Dispose() {
		if (_blockingStreams == null) return;
		foreach (var stream in _blockingStreams) {
			stream.Dispose();
		}
		_blockingStreams = null;
	}
}
