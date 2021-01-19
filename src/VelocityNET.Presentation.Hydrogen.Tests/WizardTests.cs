using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Tests
{

    public class WizardTests
    {
        [Test]
        public void WizardStart()
        {
            var wizard = new WizardViewModel<bool>
            {
                Steps = new List<Type>
                {
                    typeof(TestWizardStep),
                    typeof(TestWizardStep)
                }
            };

            Assert.AreEqual(2, wizard.Steps.Count());
            Assert.IsTrue(wizard.HasNext);
            Assert.IsFalse(wizard.HasPrevious);
        }

        [Test]
        public async Task WizardFinish()
        {
            var wizard = new WizardViewModel<int>
            {
                Model = 12345,
                Steps = new List<Type>
                {
                    typeof(TestWizardStep),
                    typeof(TestWizardStep)
                },
                CurrentStepInstance = new TestWizardStep()
            };

            bool recieved = false;
            wizard.Finished += (sender, args) => recieved = true;

            await wizard.NextAsync();
            await wizard.FinishAsync();

            Assert.IsTrue(wizard.IsFinished);
            Assert.IsTrue(recieved);
            Assert.IsFalse(wizard.HasNext);

            Assert.ThrowsAsync<InvalidOperationException>(wizard.PreviousAsync);
            Assert.ThrowsAsync<InvalidOperationException>(wizard.NextAsync);
        }
    }

    internal class TestWizardStep : WizardStep<bool, WizardStepComponentViewModelBase>
    {
        public override Task<bool> OnNextAsync() => Task.FromResult(true);

        public override Task<bool> OnPreviousAsync() => Task.FromResult(true);
    }

}