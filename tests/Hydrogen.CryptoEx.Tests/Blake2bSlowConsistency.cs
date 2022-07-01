using System;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Hydrogen;
using Hydrogen.Maths;
using Hydrogen.NUnit;

namespace Hydrogen.CryptoEx.Tests;

[TestFixture]
[Parallelizable(ParallelScope.None)]
// Tests the slow Blake2b hasher in Hydrogen proj
public class Blake2bSlowConsistency {

    [Test]
    public void Empty() {
        var slowHasher = new Hydrogen.Blake2b(Hydrogen.Blake2b._512Config);
        using var _ = Hashers.BorrowHasher(CHF.Blake2b_512_Fast, out var fastHasher);
        Assert.That(slowHasher.GetResult(), Is.EqualTo(fastHasher.GetResult()));
    }

    [Test]
    public void Empty_2() {
        var slowHasher = new Hydrogen.Blake2b(Hydrogen.Blake2b._512Config);
        using var _ = Hashers.BorrowHasher(CHF.Blake2b_512_Fast, out var fastHasher);
        slowHasher.Transform(Array.Empty<byte>());
        fastHasher.Transform(Array.Empty<byte>());
        Assert.That(slowHasher.GetResult(), Is.EqualTo(fastHasher.GetResult()));
    }


    [Test]
    public void Random() {
        var slowHasher = new Hydrogen.Blake2b(Hydrogen.Blake2b._512Config);
        using var _ = Hashers.BorrowHasher(CHF.Blake2b_512_Fast, out var fastHasher);
        var bytes = new Random(31337).NextBytes(100);
        slowHasher.Transform(bytes);
        fastHasher.Transform(bytes);
        Assert.That(slowHasher.GetResult(), Is.EqualTo(fastHasher.GetResult()));
    }



    [Test]
    public void Complex() {
        var rng = new Random(31337);
        var slowHasher = new Hydrogen.Blake2b(Hydrogen.Blake2b._512Config);
        using var _ = Hashers.BorrowHasher(CHF.Blake2b_512_Fast, out var fastHasher);

        for (var i = 0; i < rng.Next(0, 100); i++) {
            var block = rng.NextBytes(rng.Next(0, 100));
            slowHasher.Transform(block);
            fastHasher.Transform(block);
        }
        Assert.That(slowHasher.GetResult(), Is.EqualTo(fastHasher.GetResult()));
        Assert.That(slowHasher.GetResult(), Is.EqualTo(fastHasher.GetResult()));
        Assert.That(slowHasher.GetResult(), Is.EqualTo(fastHasher.GetResult()));
        Assert.That(slowHasher.GetResult(), Is.EqualTo(fastHasher.GetResult()));
        Assert.That(slowHasher.GetResult(), Is.EqualTo(fastHasher.GetResult()));
    }



}
