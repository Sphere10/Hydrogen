using System;
using System.Linq;
using AutoFixture;
using NUnit.Framework;
using VelocityNET.Presentation.Hydrogen.Components.Tables;
using VelocityNET.Presentation.Hydrogen.Models;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Tests
{

    public class PagedTableTests
    {
        private Fixture AutoFixture { get; } = new ();

        [Test]
        public void ProgressThroughPagesCorrectly()
        {
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
        public void NextAndPrevious()
        {
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
        public void HasNextAsExpected()
        {
            var vm = new PagedTableViewModel<Block>
            {
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
        public void ChangePageSizeSetsPage()
        {
            var vm = new PagedTableViewModel<Block>
            {
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
}