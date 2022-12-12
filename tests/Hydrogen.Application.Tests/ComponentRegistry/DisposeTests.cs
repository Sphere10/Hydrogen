//-----------------------------------------------------------------------
// <copyright file="UrlToolTests.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using Hydrogen.Application;
using TinyIoC;

namespace Hydrogen.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class DisposeTests {

        [Test]
        public void DoesNotDisposeExplicitInstance() {
	        // Note this required a change to TinyIoC (InstanceFactory.Dispose method, remove instance disposal)
            var disposed = false;
            var dispoable = new ActionDisposable( () => disposed = true);
            using (var componentRegistry = new ComponentRegistry()) {
                componentRegistry.RegisterComponentInstance(dispoable);
            }
            Assert.IsFalse(disposed);
        }

  
    }
}
