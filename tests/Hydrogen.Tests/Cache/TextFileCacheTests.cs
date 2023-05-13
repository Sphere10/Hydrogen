// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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
public class TextFileCacheTests {

    [Test]
    public void ReloadsStaleFile() {
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
        Thread.Sleep(1000); // allow slow file monitoring (github test runner having issues)
        Assert.That(cache[file], Is.EqualTo("BETA"));
        Assert.That(fetchedCount, Is.EqualTo(2));
        Assert.That(cache[file], Is.EqualTo("BETA"));
        Assert.That(fetchedCount, Is.EqualTo(2));
    }

    [Test]
    public void RetainsCacheWhenRetainCachedOnDeleteWhenIsTrue() {
        using var disposables = new Disposables();
        var file = Path.GetTempFileName();
        disposables.Add(() => File.Delete(file));

        File.WriteAllText(file, "ALPHA");

        var fetchedCount = 0;
        var cache = new TextFileCache { RetainCacheOnDelete = true };
        cache.ItemFetching += _ => fetchedCount++;

        Assert.That(cache[file], Is.EqualTo("ALPHA"));
        File.Delete(file);
        Assert.That(cache[file], Is.EqualTo("ALPHA"));
        Assert.That(fetchedCount, Is.EqualTo(1));
        File.WriteAllText(file, "ALPHA");
        Thread.Sleep(400); // allow slow file monitoring (github test runner having issues)
        Assert.That(cache[file], Is.EqualTo("ALPHA"));
        Assert.That(fetchedCount, Is.EqualTo(2));
    }

    [Test]
    public void ThrowsWhenRetainCachedOnDeleteIsFalse() {
        using var disposables = new Disposables();
        var file = Path.GetTempFileName();
        disposables.Add(() => File.Delete(file));

        File.WriteAllText(file, "ALPHA");
        Thread.Sleep(400); // allow slow file monitoring (github test runner having issues)

        var fetchedCount = 0;
        var cache = new TextFileCache { RetainCacheOnDelete = false };
        cache.ItemFetching += _ => fetchedCount++;

        Assert.That(cache[file], Is.EqualTo("ALPHA"));
        File.Delete(file);
        Assert.That(() => cache[file], Throws.Exception);
    }

}

