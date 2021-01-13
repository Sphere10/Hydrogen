using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using VelocityNET.Presentation.Hydrogen.Models;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Tests
{

    public class VirtualPagedTableViewModelTests
    {

        private TestDataService<Block> Service { get; } = new TestDataService<Block>();
        
        internal class TestDataService<T>
        {
            private List<T> Data { get; } = new Fixture().CreateMany<T>(2000).ToList();

            public int TotalItems => Data.Count;
            internal Task<ItemsResponse<T>> GetAsync(ItemRequest request)
            {
                return Task.FromResult(new ItemsResponse<T>(Data.Skip(request.Index + 1).Take(request.Count), Data.Count));
            }
        }
        
        [Test]
        public async Task FirstPageOnInit()
        {
            var vm = new VirtualPagedTableViewModel<Block>
            {
                ItemsProvider = Service.GetAsync,
            };

            await vm.InitAsync();
            
            Assert.AreEqual(vm.PageSize, vm.Page.Count());
            Assert.AreEqual(Service.TotalItems, vm.TotalItems);
            Assert.AreEqual((int)Math.Ceiling((double)Service.TotalItems / vm.PageSize), vm.TotalPages);
        }

        [Test]
        public async Task NextAndPrevious()
        {
            var vm = new VirtualPagedTableViewModel<Block>
            {
                ItemsProvider = Service.GetAsync,
            };

            await vm.InitAsync();

            var first = vm.Page;

            await vm.NextPageAsync();
            await vm.PrevPageAsync();
            
            Assert.AreEqual(first, vm.Page);
        }
    }
}