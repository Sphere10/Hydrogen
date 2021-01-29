using System;
using NUnit.Framework;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;

namespace VelocityNET.Presentation.Hydrogen.Tests.Wizard
{

    public class DefaultWizardTests
    {
        [Test]
        public void Initialized()
        {
            IWizard wizard =
                new DefaultWizard<TestModel>("test", new Type[] {typeof(object)}, new TestModel(), null, null);

            Assert.NotNull(wizard.CurrentStep);
            Assert.IsFalse(wizard.HasNext);
            Assert.IsFalse(wizard.HasPrevious);
        }

        [Test]
        public void NextAsync()
        {
            IWizard wizard =
                new DefaultWizard<TestModel>("test", new Type[]
                {
                    typeof(object),
                    typeof(object),
                    typeof(object)
                }, new TestModel(), null, null);

            Assert.IsTrue(wizard.HasNext);
            Assert.IsFalse(wizard.HasPrevious);

            wizard.Next();

            Assert.IsTrue(wizard.HasNext);
            Assert.IsTrue(wizard.HasPrevious);
            Assert.NotNull(wizard.CurrentStep);

            wizard.Next();

            Assert.IsFalse(wizard.HasNext);
            Assert.IsTrue(wizard.HasPrevious);
            Assert.NotNull(wizard.CurrentStep);
        }
    }

}