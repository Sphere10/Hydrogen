//-----------------------------------------------------------------------
// <copyright file="SessionCacheTests.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class TextFileCacheTests
{

    [Test]
    public void Simple()
    {
        using var disposables = new Disposables();
        var file = Path.GetTempFileName();
        disposables.Add(() => File.Delete(file));

        File.WriteAllText(file, "ALPHA");

        var fetchedCount = 0;
        var cache = new TextFileCache();
        cache.ItemFetching += _ => fetchedCount++;

        Assert.That(cache[file], Is.EqualTo("ALPHA"));
        Assert.That(fetchedCount, Is.EqualTo(1));
        Assert.That(cache[file], Is.EqualTo("ALPHA"));
        Assert.That(fetchedCount, Is.EqualTo(1));

        File.WriteAllText(file, "BETA");
        Assert.That(cache[file], Is.EqualTo("BETA"));
        Assert.That(fetchedCount, Is.EqualTo(2));
        Assert.That(cache[file], Is.EqualTo("BETA"));
        Assert.That(fetchedCount, Is.EqualTo(2));

    }


}

