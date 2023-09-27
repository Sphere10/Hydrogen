// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using AutoFixture;
using NUnit.Framework;
using Hydrogen.DApp.Presentation.Components.Tables;
using Hydrogen.DApp.Presentation.Models;

namespace Hydrogen.DApp.Presentation.Tests;

public class PagedTableTests {
	private Fixture AutoFixture { get; } = new();

	[Test]
	public void ProgressThroughPagesCorrectly() {
		var vm = new PagedTableViewModel<Block>();

		int pageSize = 5;
		int rowCount = 14;

		vm.PageSize = pageSize;
		vm.Items = AutoFixture.CreateMany<Block>(rowCount).ToList();

		Assert.AreEqual(5, vm.Page.Count());

		vm.NextPageAsync();
		Assert.AreEqual(2, vm.CurrentPage);
		Assert.AreEqual(5, vm.Page.Count());

		vm.NextPageAsync();
		Assert.AreEqual(3, vm.CurrentPage);
		Assert.AreEqual(4, vm.Page.Count());

		Assert.ThrowsAsync<InvalidOperationException>(vm.NextPageAsync);
	}

	[Test]
	public void NextAndPrevious() {
		var vm = new PagedTableViewModel<Block>();

		int pageSize = 5;
		int rowCount = 15;

		vm.PageSize = pageSize;
		vm.Items = AutoFixture.CreateMany<Block>(rowCount).ToList();

		var first = vm.Page;

		vm.NextPageAsync();
		vm.NextPageAsync();
		vm.PrevPageAsync();
		vm.PrevPageAsync();

		Assert.AreEqual(first, vm.Page);
	}

	[Test]
	public void HasNextAsExpected() {
		var vm = new PagedTableViewModel<Block> {
			PageSize = 3,
			Items = AutoFixture.CreateMany<Block>(9).ToList()
		};

		Assert.IsTrue(vm.HasNextPage);
		Assert.IsFalse(vm.HasPrevPage);

		vm.NextPageAsync();

		Assert.IsTrue(vm.HasNextPage);
		Assert.IsTrue(vm.HasPrevPage);

		vm.NextPageAsync();

		Assert.IsFalse(vm.HasNextPage);
		Assert.IsTrue(vm.HasPrevPage);
	}

	[Test]
	public void ChangePageSizeSetsPage() {
		var vm = new PagedTableViewModel<Block> {
			PageSize = 1,
			Items = AutoFixture.CreateMany<Block>(10).ToList()
		};

		Assert.AreEqual(10, vm.TotalPages);
		Assert.AreEqual(1, vm.CurrentPage);

		vm.NextPageAsync();

		Assert.AreEqual(2, vm.CurrentPage);

		vm.PageSize = 3;

		Assert.AreEqual(1, vm.CurrentPage);
	}
}
