using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using VelocityNET.Presentation.Hydrogen.Components;
using VelocityNET.Presentation.Hydrogen.Components.Tables;

namespace VelocityNET.Presentation.Hydrogen.Tests {

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

            Assert.AreEqual(10, vm.Items.Count);
        }

        [Test]
        public async Task TotalItemsLimitsItems() {
            var vm = new RapidTableViewModel<bool> {
                Source = DataSource(),
                ItemLimit = 2
            };

            await vm.InitAsync();
            await Task.Delay(10);

            Assert.AreEqual(2, vm.Items.Count);
        }
    }

}