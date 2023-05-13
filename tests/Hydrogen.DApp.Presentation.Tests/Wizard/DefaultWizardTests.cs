// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Hydrogen;
using Hydrogen.DApp.Presentation.Components.Wizard;

namespace Hydrogen.DApp.Presentation.Tests.Wizard {

    public class DefaultWizardTests {
        [Test]
        public void Initialized() {
            IWizard wizard =
                new DefaultWizard<bool>("test", new List<Type> { typeof(object) }, true, null, null);

            Assert.NotNull(wizard.CurrentStep);
            Assert.IsFalse(wizard.HasNext);
            Assert.IsFalse(wizard.HasPrevious);
        }

        [Test]
        public void NextAsync() {
            IWizard wizard =
                new DefaultWizard<bool>("test", new List<Type> { typeof(object), typeof(object), typeof(object) },
                    true, null, null);

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

        [Test]
        public void InjectStep() {
            IWizard wizard =
                new DefaultWizard<object>("Test", new List<Type> { typeof(int) }, new object(), null, null);


            Assert.AreEqual(typeof(int), wizard.CurrentStep);
            wizard.Next();

            Assert.IsFalse(wizard.HasNext);
            wizard.UpdateSteps(StepUpdateType.Inject, new[] { typeof(double) });
            Assert.IsTrue(wizard.HasNext);

            bool result = wizard.Next();
            Assert.IsTrue(result);
            Assert.AreEqual(typeof(double), wizard.CurrentStep);
            Assert.IsFalse(wizard.HasNext);
        }

        [Test]
        public void InjectStepTwiceDedupe() {
            IWizard wizard =
                new DefaultWizard<object>("Test", new List<Type> { typeof(int) }, new object(), null, null);

            Assert.AreEqual(typeof(int), wizard.CurrentStep);
            Assert.IsFalse(wizard.HasNext);

            wizard.UpdateSteps(StepUpdateType.Inject, new[] { typeof(double) });
            wizard.UpdateSteps(StepUpdateType.Inject, new[] { typeof(double) });
            Assert.IsTrue(wizard.HasNext);

            bool result = wizard.Next();
            bool secondResult = wizard.Next();

            Assert.IsTrue(result);
            Assert.IsFalse(secondResult);
            Assert.AreEqual(typeof(double), wizard.CurrentStep);
            Assert.IsFalse(wizard.HasNext);
        }

        [Test]
        public void ReplaceAllNextSteps() {
            IWizard wizard =
                new DefaultWizard<object>("Test", new List<Type> { typeof(int), typeof(decimal), typeof(double) },
                    new object(), null, null);

            wizard.UpdateSteps(StepUpdateType.ReplaceAllNext, new[] { typeof(bool) });

            bool result = wizard.Next();
            Assert.IsTrue(result);
            Assert.AreEqual(typeof(bool), wizard.CurrentStep);
            Assert.IsFalse(wizard.HasNext);
        }

        [Test]
        public void RemoveNext() {
            IWizard wizard =
                new DefaultWizard<object>("Test", new List<Type> { typeof(int), typeof(decimal), typeof(double) },
                    new object(), null, null);

            wizard.UpdateSteps(StepUpdateType.RemoveNext, new[] { typeof(decimal), typeof(double) });

            Assert.IsFalse(wizard.HasNext);
            Assert.AreEqual(typeof(int), wizard.CurrentStep);
        }

        [Test]
        public void ReplaceAll() {
            IWizard wizard =
                new DefaultWizard<object>("Test", new List<Type> { typeof(int) },
                    new object(), null, null);

            wizard.UpdateSteps(StepUpdateType.ReplaceAll, new[] { typeof(decimal), typeof(double) });

            Assert.AreEqual(typeof(decimal), wizard.CurrentStep);
            Assert.IsTrue(wizard.HasNext);
        }

        [Test]
        public async Task FinishAsyncFalse() {
            IWizard wizard =
                new DefaultWizard<bool>("Test", new List<Type> { typeof(int) },
                    false, x => Task.FromResult<Result<bool>>(x), null);

            bool result = await wizard.FinishAsync();
            Assert.IsFalse(result);
        }

        [Test]
        public async Task FinishAsyncTrue() {
            IWizard wizard =
                new DefaultWizard<bool>("Test", new List<Type> { typeof(int) },
                    true, x => Task.FromResult<Result<bool>>(x), null);

            bool result = await wizard.FinishAsync();
            Assert.IsTrue(result);
        }

        [Test]
        public async Task CancelAsyncFalse() {
            IWizard wizard =
                new DefaultWizard<bool>("Test", new List<Type> { typeof(int) },
                    false, null, x => Task.FromResult<Result<bool>>(x));

            bool result = await wizard.CancelAsync();
            Assert.IsFalse(result);
        }

        [Test]
        public async Task CancelAsyncTrue() {
            IWizard wizard =
                new DefaultWizard<bool>("Test", new List<Type> { typeof(int) },
                    true, null, x => Task.FromResult<Result<bool>>(x));

            bool result = await wizard.CancelAsync();
            Assert.IsTrue(result);
        }
    }

}