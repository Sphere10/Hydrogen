// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen.Windows.Forms;
using Exception = System.Exception;

namespace Hydrogen.Utils.WinFormsTester;

public partial class TransactionalCollectionScreen : ApplicationScreen {
	private TextWriter _outputWriter;

	public TransactionalCollectionScreen() {
		InitializeComponent();
		_listTypeComboBox.EnumType = typeof(ListType);
		_listTypeComboBox.SelectedEnum = ListType.Transactional;
		_outputWriter = new TextBoxWriter(_outputTextBox);
		_copyButton.Image = Hydrogen.Windows.Forms.Resources.Copy_16x16.ToBitmap(16, 16);
		_clearButton.Image = Hydrogen.Windows.Forms.Resources.CrossIcon.ToBitmap(16, 16);
		_policyBox.EnumType = typeof(ClusteredStreamsPolicy);
		_policyBox.SelectedEnum = ClusteredStreamsPolicy.Default;
	}


	private void RunListAppendTest() {
		var rng = new Random(31337);
		var stats = new Statistics();
		var totalTime = TimeSpan.Zero;

		using var _ = CreateList(out var list);

		var itemCount = _itemsIntBox.Value.GetValueOrDefault(0);
		var batchSize = _batchIntBox.Value.GetValueOrDefault(0);
		var itemSize = _itemSizeIntBox.Value.GetValueOrDefault(0);
		var commit = _commitCheckBox.Checked && list is ITransactionalObject;
		var totalBytes = 0;
		foreach (var batch in Tools.Collection.Partition(itemCount, batchSize)) {
			var batchDuration = Do(() => {
				var items = Tools.Collection.Generate(() => new byte[itemSize]).Take(batch);
				totalBytes += batch * itemSize;
				list.AddRange(items);
			});
			totalTime += batchDuration;
			var perItemAverageDuration = batchDuration / batch;
			stats.AddDatum(perItemAverageDuration.TotalMilliseconds, batch);
			_outputWriter.WriteLine(
				$"Appended {batch}, Total Duration (ms): {batchDuration.TotalMilliseconds:#.###}, Batch Avg: {perItemAverageDuration.TotalMilliseconds / batch:#.###}, Total Bytes = {Tools.Memory.ConvertToReadable(totalBytes, MemoryMetric.Byte)}");
			if (commit)
				((ITransactionalObject)list).Commit();
		}
		_outputWriter.WriteLine($"Total: {stats.Sum:#.##} (ms), Avg (ms): {stats.Mean:#.###}, Total Bytes = {Tools.Memory.ConvertToReadable(totalBytes, MemoryMetric.Byte)}");

		TimeSpan Do(Action action) {
			var start = DateTime.Now;
			action();
			return DateTime.Now - start;
		}
	}

	private void RunBufferAppendTest() {
		var rng = new Random(31337);
		var stats = new Statistics();
		var totalTime = TimeSpan.Zero;

		using var _ = CreateBuffer(out var list);

		var itemCount = _itemsIntBox.Value.GetValueOrDefault(0);
		var batchSize = _batchIntBox.Value.GetValueOrDefault(0);
		var itemSize = _itemSizeIntBox.Value.GetValueOrDefault(0);
		var commit = _commitCheckBox.Checked && list is ITransactionalObject;
		;
		var totalBytes = 0;
		foreach (var batch in Tools.Collection.Partition(itemCount, batchSize)) {
			var batchDuration = Do(() => {
				var items = new byte[itemSize];
				for (var i = 0; i < batch; i++) {
					totalBytes += items.Length;
					list.AddRange(items);
				}
			});
			totalTime += batchDuration;
			var perItemAverageDuration = batchDuration / batch;
			stats.AddDatum(perItemAverageDuration.TotalMilliseconds, batch);
			_outputWriter.WriteLine(
				$"Appended {batch}, Total Duration (ms): {batchDuration.TotalMilliseconds:#.###}, Batch Avg: {perItemAverageDuration.TotalMilliseconds / batch:#.###}, Total Bytes = {Tools.Memory.ConvertToReadable(totalBytes, MemoryMetric.Byte)}");
			if (commit)
				((ITransactionalObject)list).Commit();
		}
		_outputWriter.WriteLine($"Total: {stats.Sum:#.##} (ms), Avg (ms): {stats.Mean:#.###}, Total Bytes = {Tools.Memory.ConvertToReadable(totalBytes, MemoryMetric.Byte)}");

		TimeSpan Do(Action action) {
			var start = DateTime.Now;
			action();
			return DateTime.Now - start;
		}
	}


	private void RunStreamTest(int clusterSize, int pageSize, int maxMemory, ClusteredStreamsPolicy policy) {
		var file = Path.GetTempFileName();
		using var _ = Tools.Scope.ExecuteOnDispose(() => File.Delete(file));
		using (var transactionalFile = new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From(file, pageSize, maxMemory))) {
			using var rootStream = new ExtendedMemoryStream(transactionalFile);
			if (rootStream.RequiresLoad)
				rootStream.Load();

			var storage = new ClusteredStreams(rootStream, clusterSize, policy: policy);
			var rng = new Random(31337);
			var stats = new Statistics();

			var totalTime = TimeSpan.Zero;
			var totalbytes = Tools.Memory.ConvertMemoryMetric(10, MemoryMetric.Megabyte, MemoryMetric.Byte);


			using (var scope = storage.Add()) {
				var bytes = rng.NextBytes(65536);
				while (totalbytes > 0) {
					var time = Do(() => scope.WriteBytes(bytes));
					_outputWriter.WriteLine($"Appended {bytes.Length} b, Chunk Duration (ms): {time.TotalMilliseconds:#.###}");
					totalTime += time;
					totalbytes -= bytes.Length;
				}
				_outputWriter.WriteLine($"Total {Tools.Memory.ConvertMemoryMetric(10, MemoryMetric.Megabyte, MemoryMetric.Byte)} b, Total Time (sec): {totalTime.TotalSeconds:#.###}");
			}
		}

		TimeSpan Do(Action action) {
			var start = DateTime.Now;
			action();
			return DateTime.Now - start;
		}
	}

	private void RunDictAppendTest() {
		var rng = new Random(31337);
		var totalTime = TimeSpan.Zero;
		var policy = (ClusteredStreamsPolicy)_policyBox.SelectedEnum;
		var dict = new TransactionalDictionary<byte[], byte[]>(
			HydrogenFileDescriptor.From(
				Path.GetTempFileName(),
				Path.GetTempPath(),
				pageSize: _pageSizeIntBox.Value.GetValueOrDefault(0),
				maxMemory: _cacheSizeIntBox.Value.GetValueOrDefault(0),
				containerPolicy: policy,
				clusterSize: _clusterSizeIntBox.Value.GetValueOrDefault(0)
			),
			ByteArraySerializer.Instance,
			ByteArraySerializer.Instance
		);

		var itemCount = _itemsIntBox.Value.GetValueOrDefault(0);
		var batchSize = _batchIntBox.Value.GetValueOrDefault(0);
		var itemSize = _itemSizeIntBox.Value.GetValueOrDefault(0);
		var commit = _commitCheckBox.Checked;
		var stats = new Statistics();
		var batchStats = new Statistics();
		foreach (var batch in Tools.Collection.Partition(itemCount, batchSize)) {
			for (var i = 0; i < batch; i++) {
				batchStats.Reset();
				var key = rng.NextBytes(32);
				var value = rng.NextBytes(itemSize);
				var duration = Do(() => dict.Add(key, value));
				batchStats.AddDatum(duration.TotalMilliseconds);
				stats.AddDatum(duration.TotalMilliseconds);
				if (commit)
					dict.Commit();
			}
			_outputWriter.WriteLine($"Appended {batch}, Batch Duration (ms): {batchStats.Sum:#.###}, Batch Avg: {batchStats.Mean:#.###}, Size = {Tools.Memory.ConvertToReadable(dict.ObjectStream.Streams.RootStream.Length, MemoryMetric.Byte)}");
		}
		_outputWriter.WriteLine($"Total: {stats.Sum:#.##} (ms), Avg (ms): {stats.Mean:#.###}, Size = {Tools.Memory.ConvertToReadable(dict.ObjectStream.Streams.RootStream.Length, MemoryMetric.Byte)}");

		TimeSpan Do(Action action) {
			var start = DateTime.Now;
			action();
			return DateTime.Now - start;
		}
	}

	private IDisposable CreateList(out IExtendedList<byte[]> list) {
		var disposables = new Disposables();
		list = default;
		var listType = this.Invoke(() => (ListType)_listTypeComboBox.SelectedEnum);
		var pageSize = _pageSizeIntBox.Value.GetValueOrDefault(0);
		var clusterSize = _clusterSizeIntBox.Value.GetValueOrDefault(0);
		var maxMemory = _cacheSizeIntBox.Value.GetValueOrDefault(0);
		var policy = (ClusteredStreamsPolicy)_policyBox.SelectedEnum;
		switch (listType) {
			case ListType.Transactional:
				var txnList = new TransactionalList<byte[]>(
					HydrogenFileDescriptor.From(
						Path.GetTempFileName(),
						Path.GetTempPath(),
						pageSize: pageSize,
						maxMemory: maxMemory,
						containerPolicy: policy,
						clusterSize: clusterSize
					),
					new ByteArraySerializer()
				);
				disposables.Add(txnList);
				list = txnList;
				break;
			case ListType.MerkleizedList:
				var filename = Path.GetTempFileName();
				disposables.Add(() => File.Delete(filename));
				Stream stream = new ExtendedMemoryStream(new FileMappedBuffer(TransactionalFileDescriptor.From(filename, pageSize, maxMemory: maxMemory)), true);
				var merkleList = new StreamMappedMerkleList<byte[]>(
					stream,
					CHF.SHA2_256,
					clusterSize
				);
				disposables.Add(stream);
				list = merkleList;
				break;
			default:
				//Stream stream = default;
				switch (listType) {
					case ListType.MemoryStream:
						stream = new MemoryStream();
						break;
					case ListType.MemoryBuffer:
						stream = new ExtendedMemoryStream(new MemoryBuffer(pageSize, pageSize), true);
						break;
					case ListType.MemoryPagedBuffer:
						stream = new ExtendedMemoryStream(new MemoryPagedBuffer(pageSize, maxMemory: maxMemory), true);
						break;
					case ListType.FilePaged:
						filename = Path.GetTempFileName();
						disposables.Add(() => File.Delete(filename));
						stream = new ExtendedMemoryStream(new FileMappedBuffer(TransactionalFileDescriptor.From(filename, pageSize, maxMemory: maxMemory)), true);
						break;
					case ListType.TransactionalFilePaged:
						var path = Tools.FileSystem.GetTempEmptyDirectory(true);
						filename = Path.GetTempFileName();
						disposables.Add(() => File.Delete(filename));
						disposables.Add(() => Tools.FileSystem.DeleteDirectories(path));
						stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From(filename, path, pageSize, maxMemory: maxMemory)), true);
						break;
					default:
						throw new NotSupportedException(listType.ToString());
				}
				var clusteredList = StreamMappedFactory.CreateList(
					stream,
					clusterSize,
					new ByteArraySerializer(),
					policy: policy
				);
				disposables.Add(stream);
				list = clusteredList;
				break;
		}
		return disposables;
	}

	private IDisposable CreateBuffer(out IBuffer buffer) {
		var disposables = new Disposables();
		buffer = default;
		var listType = this.Invoke(() => (ListType)_listTypeComboBox.SelectedEnum);
		var pageSize = _pageSizeIntBox.Value.GetValueOrDefault(0);
		var clusterSize = _clusterSizeIntBox.Value.GetValueOrDefault(0);
		var maxMemory = _cacheSizeIntBox.Value.GetValueOrDefault(0);
		switch (listType) {
			case ListType.Transactional:
				throw new NotSupportedException();
			case ListType.MemoryStream:
				throw new NotSupportedException();
			case ListType.MemoryBuffer:
				buffer = new MemoryBuffer(pageSize, pageSize);
				break;
			case ListType.MemoryPagedBuffer:
				buffer = new MemoryPagedBuffer(pageSize, maxMemory);
				break;
			case ListType.FilePaged:
				var filename = Path.GetTempFileName();
				disposables.Add(() => File.Delete(filename));
				buffer = new FileMappedBuffer(TransactionalFileDescriptor.From(filename, pageSize, maxMemory: maxMemory));
				break;
			case ListType.TransactionalFilePaged:
				var path = Tools.FileSystem.GetTempEmptyDirectory(true);
				filename = Path.GetTempFileName();
				disposables.Add(() => File.Delete(filename));
				disposables.Add(() => Tools.FileSystem.DeleteDirectories(path));
				buffer = new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From(filename, path, pageSize, maxMemory: maxMemory));
				break;
			default:
				throw new NotSupportedException(listType.ToString());
		}
		return disposables;
	}

	private void _clearButton_Click(object sender, EventArgs e) {
		try {
			_outputTextBox.Clear();
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void _copyButton_Click(object sender, EventArgs e) {
		try {
			Clipboard.SetText(_outputTextBox.Text);
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private async void _appendTestButton_Click(object sender, EventArgs e) {
		try {
			await Task.Run(RunListAppendTest);
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	private async void _streamButton_Click(object sender, EventArgs e) {
		try {
			await Task.Run(() => RunStreamTest(
				_clusterSizeIntBox.Value.GetValueOrDefault(0),
				_pageSizeIntBox.Value.GetValueOrDefault(0),
				_cacheSizeIntBox.Value.GetValueOrDefault(0),
				(ClusteredStreamsPolicy)_policyBox.SelectedEnum
			));
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	private async void _dictionaryAppend_Click(object sender, EventArgs e) {
		try {
			await Task.Run(RunDictAppendTest);
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	private async void _bufferTestButton_Click(object sender, EventArgs e) {
		try {
			await Task.Run(RunBufferAppendTest);
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}


	public enum ListType {
		Transactional,
		MemoryStream,
		MemoryBuffer,
		MemoryPagedBuffer,
		FilePaged,
		TransactionalFilePaged,
		MerkleizedList,
	}


}
