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
using Hydrogen.DApp.Presentation.Components.Tables;
using NUnit.Framework.Legacy;

namespace Hydrogen.DApp.Presentation.Tests;

public class RapidTableViewModelTests {
#pragma warning disable 1998
	private async IAsyncEnumerable<bool> DataSource()
#pragma warning restore 1998
	{
		for (int i = 0; i < 10; i++) {
			yield return true;
		}
	}

	[Test]
	public async Task InitVmPopulatesItems() {
		var vm = new RapidTableViewModel<bool> {
			Source = DataSource()
		};

		await vm.InitAsync();
		await Task.Delay(10);

		ClassicAssert.AreEqual(10, vm.Items.Count);
	}

	[Test]
	public async Task TotalItemsLimitsItems() {
		var vm = new RapidTableViewModel<bool> {
			Source = DataSource(),
			ItemLimit = 2
		};

		await vm.InitAsync();
		await Task.Delay(10);

		ClassicAssert.AreEqual(2, vm.Items.Count);
	}
}
