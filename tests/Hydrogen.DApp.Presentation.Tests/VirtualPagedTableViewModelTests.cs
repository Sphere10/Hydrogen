using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using Hydrogen.DApp.Presentation.Components;
using Hydrogen.DApp.Presentation.Components.Tables;
using Hydrogen.DApp.Presentation.Models;
using Hydrogen.DApp.Presentation.ViewModels;

namespace Hydrogen.DApp.Presentation.Tests {

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

            Assert.AreEqual(vm.PageSize, vm.Page.Count());
            Assert.AreEqual(service.TotalItems, vm.TotalItems);
            Assert.AreEqual((int)Math.Ceiling((double)service.TotalItems / vm.PageSize), vm.TotalPages);
        }

        [Test]
        public async Task NextAndPrevious() {
            var service = new TestDataService<Block>(10);

            var vm = new VirtualPagedTableViewModel<Block> {
                ItemsProvider = service.GetAsync,
                PageSize = 3
            };

            await vm.InitAsync();

            Assert.AreEqual(4, vm.TotalPages);
            Assert.AreEqual(10, vm.TotalItems);

            await vm.NextPageAsync();
            await vm.NextPageAsync();
            await vm.NextPageAsync();
            await vm.PrevPageAsync();
            await vm.PrevPageAsync();
            await vm.PrevPageAsync();

            Assert.AreEqual(1, vm.CurrentPage);
        }

        [Test]
        public async Task ChangePageSizeSetsPage() {
            var service = new TestDataService<Block>(10);

            var vm = new VirtualPagedTableViewModel<Block> {
                ItemsProvider = service.GetAsync,
                PageSize = 1
            };

            await vm.InitAsync();

            Assert.AreEqual(10, vm.TotalPages);
            Assert.AreEqual(1, vm.CurrentPage);

            await vm.NextPageAsync();

            Assert.AreEqual(2, vm.CurrentPage);

            await vm.SetPageSizeAsync(3);

            Assert.AreEqual(1, vm.CurrentPage);
        }
    }
}