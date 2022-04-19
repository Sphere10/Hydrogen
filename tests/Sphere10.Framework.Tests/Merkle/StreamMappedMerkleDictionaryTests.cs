using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Sphere10.Framework.NUnit;
using Tools;

namespace Sphere10.Framework.Tests {


	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class StreamMappedMerkleDictionaryTests : StreamMappedMerkleDictionaryTestsBase {
		private const int DefaultClusterSize = 256;

		protected override IDisposable CreateDictionary(CHF chf, out IMerkleDictionary<string, TestObject> merkleDictionary) {
			var memoryStream = new MemoryStream();
			merkleDictionary = new StreamMappedMerkleDictionary<string, TestObject>(
				memoryStream, 
				DefaultClusterSize,
				chf,
				valueComparer: new TestObjectComparer(),
				valueSerializer: new TestObjectSerializer()
			);
			return memoryStream;;
		}
	}

}
