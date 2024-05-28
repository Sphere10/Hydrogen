using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FastSerialization;
using Hydrogen.NUnit;
using Hydrogen.Tests.Collections.StreamMapped;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public sealed class TransactionalDictionaryCLKTests : TransactionalDictionaryTestsBase {

	[Test]
	public void BrokenKeySerializerFailsGracefully([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {

		var rng = new Random(31337);
		var key = rng.NextString(256 - sizeof(Int32));

		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
		using var disposable1 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file)));
		using var disposable2 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir)));

		using var dictionary = new TransactionalDictionary<string, TestObject>(
			HydrogenFileDescriptor.From(
				file,
				dir,
				containerPolicy:policy
			),
			new BrokenConstantLengthSerializer<string>(new PaddedSerializer<string>(256, new StringSerializer(Encoding.UTF8, SizeDescriptorStrategy.UseUInt32))),
			new TestObjectSerializer().AsNullableSerializer(),
			null,
			EqualityComparer<string>.Default,
			new TestObjectEqualityComparer(),
			implementation: StreamMappedDictionaryImplementation.ConstantLengthKeyBased
		);

		dictionary.Load();
		Assert.That(() =>dictionary.Add(key, new TestObject(rng)), Throws.InvalidOperationException);
	}


	protected override IDisposable Create<TKey, TValue>(IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, ClusteredStreamsPolicy policy,
	                                           out ITransactionalDictionary<TKey, TValue> clustered, out string file) {
		file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var fn = file;
		var disposable1 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(fn)));
		var disposable2 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir)));
		clustered = new TransactionalDictionary<TKey, TValue>( HydrogenFileDescriptor.From(file, dir, containerPolicy: policy),  keySerializer.AsConstantSize(256), valueSerializer, null, keyComparer, valueComparer, accessMode: FileAccessMode.OpenOrCreate);
		return new Disposables(disposable1, disposable2, clustered);
	}

}
