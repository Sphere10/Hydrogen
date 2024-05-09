// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using Hydrogen.DApp.Presentation.Components.Tables;
using Hydrogen.DApp.Presentation.Models;
using NUnit.Framework.Legacy;

namespace Hydrogen.DApp.Presentation.Tests;

public class VirtualPagedTableViewModelTests {

	internal class TestDataService<T> {
		public TestDataService(int items) {
			Data = new Fixture().CreateMany<T>(items).ToList();
		}

		private List<T> Data { get; }

		public int TotalItems => Data.Count;
		internal Task<ItemsResponse<T>> GetAsync(ItemRequest request) {
			return Task.FromResult(new ItemsResponse<T>(Data.Skip(request.Index + 1).Take(request.Count), Data.Count));
		}
	}


	[Test]
	public async Task FirstPageOnInit() {
		var service = new TestDataService<Block>(25);
		var vm = new VirtualPagedTableViewModel<Block> {
			ItemsProvider = service.GetAsync,
			PageSize = 5
		};

		await vm.InitAsync();

		ClassicAssert.AreEqual(vm.PageSize, vm.Page.Count());
		ClassicAssert.AreEqual(service.TotalItems, vm.TotalItems);
		ClassicAssert.AreEqual((int)Math.Ceiling((double)service.TotalItems / vm.PageSize), vm.TotalPages);
	}

	[Test]
	public async Task NextAndPrevious() {
		var service = new TestDataService<Block>(10);

		var vm = new VirtualPagedTableViewModel<Block> {
			ItemsProvider = service.GetAsync,
			PageSize = 3
		};

		await vm.InitAsync();

		ClassicAssert.AreEqual(4, vm.TotalPages);
		ClassicAssert.AreEqual(10, vm.TotalItems);

		await vm.NextPageAsync();
		await vm.NextPageAsync();
		await vm.NextPageAsync();
		await vm.PrevPageAsync();
		await vm.PrevPageAsync();
		await vm.PrevPageAsync();

		ClassicAssert.AreEqual(1, vm.CurrentPage);
	}

	[Test]
	public async Task ChangePageSizeSetsPage() {
		var service = new TestDataService<Block>(10);

		var vm = new VirtualPagedTableViewModel<Block> {
			ItemsProvider = service.GetAsync,
			PageSize = 1
		};

		await vm.InitAsync();

		ClassicAssert.AreEqual(10, vm.TotalPages);
		ClassicAssert.AreEqual(1, vm.CurrentPage);

		await vm.NextPageAsync();

		ClassicAssert.AreEqual(2, vm.CurrentPage);

		await vm.SetPageSizeAsync(3);

		ClassicAssert.AreEqual(1, vm.CurrentPage);
	}
}
