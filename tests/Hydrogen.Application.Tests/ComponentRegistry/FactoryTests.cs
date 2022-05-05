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
	public class FactoryTests {

        [Test]
        public void RegisterFactory() {
	        var count = 0;
	        var reg = new ComponentRegistry(new TinyIoCContainer());
            reg.RegisterComponentFactory<ISampleInterface>(r => {
	            count++;
	            return new SampleImplementation();
            });
            Assert.AreEqual(0, count);
            Assert.IsTrue(reg.HasImplementationFor<ISampleInterface>());
            Assert.IsFalse(reg.HasImplementation<Test>());
        }

        [Test]
        public void ResolveFactory() {
	        var count = 0;
	        var reg = new ComponentRegistry(new TinyIoCContainer());
	        reg.RegisterComponentFactory<ISampleInterface>(r => {
		        count++;
		        return new SampleImplementation();
	        });
	        var impl = reg.Resolve<ISampleInterface>();
	        Assert.AreEqual(1, count);
			Assert.IsTrue(impl.GetType() == typeof(SampleImplementation));
        }

        public interface ISampleInterface {
	        void Foo();
        }


        public class SampleImplementation : ISampleInterface {

	        public void Foo() {
		        throw new NotImplementedException();
	        }
        }

    }
}
