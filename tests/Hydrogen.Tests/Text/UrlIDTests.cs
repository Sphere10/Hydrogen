// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class UrlIDTests {


	[Test]
	public async Task PermiateAll() {
		var permutes = new HashSet<uint>();

		using (var queue = new ProducerConsumerQueue<uint>(uint.MaxValue)) {
			var permuteTask = new Task(() => {
				for (long i = uint.MinValue; i < 10000000; i++) {
					queue.Put(UrlID.PermuteId((uint)i));
					if (i % 1000000 == 0)
						System.Console.WriteLine("Processed {0}", i);

				}
				queue.FinishedProducing();
			});

			var checkTask = new Task(() => {
				while (!queue.HasFinishedProducing) {
					var toCheck = queue.Take();
					ClassicAssert.AreEqual(false, permutes.Contains(toCheck), "Value {0} clashed".FormatWith(toCheck));
					permutes.Add(toCheck);
				}
			});

			permuteTask.Start();
			checkTask.Start();
			await Task.WhenAll(permuteTask, checkTask);
		}
	}

}
