using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydrogen.ObjectSpaces;

namespace Hydrogen.Tests.ObjectSpaces;

public static class TestsHelper {


	public static ObjectSpace CreateObjectSpace(TestTraits traits, Dictionary<string, object> activationArgs = default, IndexNullPolicy nullPolicy = IndexNullPolicy.IgnoreNull) {
		activationArgs ??= [];
		var disposables = new Disposables();

		var builder = new ObjectSpaceBuilder();
		builder
			.AutoLoad()
			.AddDimension<Account>(true)
				.WithUniqueIndexOn(x => x.Name, nullPolicy: nullPolicy)
				.WithUniqueIndexOn(x => x.UniqueNumber, nullPolicy: nullPolicy)
				.WithChangeTrackingVia(x => x.Dirty)
				.UsingEqualityComparer(CreateAccountComparer())
				.Done()
			.AddDimension<Identity>(true)
				.WithUniqueIndexOn(x => x.Key)
				.WithChangeTrackingVia(x => x.Dirty)
				.UsingEqualityComparer(CreateIdentityComparer())
			.Done();

		// PI 
		if (traits.HasFlag(TestTraits.PersistentIgnorant))
			builder.AutoSave();

		// memory mapped
		if (traits.HasFlag(TestTraits.MemoryMapped)) { 
			if (!activationArgs.TryGetValue("stream", out var stream)) {
				stream = new MemoryStream();
				disposables.Add((MemoryStream)stream);
			}
			builder.UseMemoryStream((MemoryStream)stream);
			 
		}

		// file mapped
		if (traits.HasFlag(TestTraits.FileMapped)) {
			if (!activationArgs.TryGetValue("folder", out var folder)) {
				folder = Tools.FileSystem.GetTempEmptyDirectory(true);
				disposables.Add(Tools.Scope.DeleteDirOnDispose((string)folder));
			}
			var file = Path.Combine((string)folder, "app.db");
			builder.UseFile(file);
		}

		// merkleized
		if (traits.HasFlag(TestTraits.Merklized))
			builder.Merkleized();

		var objectSpace = builder.Build();
		objectSpace.Disposables.Add(disposables);
		return objectSpace;
	}

	public static Account CreateAccount(Random rng = null) {
		rng ??= new Random(31337);
		var secret = "MyPassword";

		var dss = DSS.PQC_WAMSSharp;
		var privateKey = Signers.CreatePrivateKey(dss, Hashers.Hash(CHF.SHA2_256, secret.ToAsciiByteArray()));
		var publicKey = Signers.DerivePublicKey(dss, privateKey, 0);

		var identity = new Identity {
			DSS = dss,
			Key = publicKey.RawBytes
		};

		var account = new Account {
			Identity = identity,
			Name = $"Savings {rng.NextInt64()}",
			UniqueNumber = rng.NextInt64(),
			Quantity = 0
		};

		return account;
	}

	public static IEqualityComparer<Account> CreateAccountComparer() 
		=> EqualityComparerBuilder
			.For<Account>()
			.By(x => x.Name)
			.ThenBy(x => x.Quantity)
			.ThenBy(x => x.Identity, CreateIdentityComparer());

	public static IEqualityComparer<Identity> CreateIdentityComparer() 
		=> EqualityComparerBuilder
			.For<Identity>()
			.By(x => x.DSS)
			.ThenBy(x => x.Key, ByteArrayEqualityComparer.Instance);



	public static readonly IEnumerable<TestTraits> AllTestCases = [
		// FileMapped combinations
		TestTraits.FileMapped,
		TestTraits.FileMapped | TestTraits.Merklized,
		TestTraits.FileMapped | TestTraits.PersistentIgnorant,
		TestTraits.FileMapped | TestTraits.Merklized | TestTraits.PersistentIgnorant,

		// MemoryMapped combinations
		TestTraits.MemoryMapped,
		TestTraits.MemoryMapped | TestTraits.Merklized,
		TestTraits.MemoryMapped | TestTraits.PersistentIgnorant,
		TestTraits.MemoryMapped | TestTraits.Merklized | TestTraits.PersistentIgnorant
	];

	public static readonly TestTraits[] FileMappedTestCases = {
		TestTraits.FileMapped,
		TestTraits.FileMapped | TestTraits.Merklized,
		TestTraits.FileMapped | TestTraits.PersistentIgnorant,
		TestTraits.FileMapped | TestTraits.Merklized | TestTraits.PersistentIgnorant
	};

	public static readonly IEnumerable<TestTraits> MemoryMappedTestCases = [
		TestTraits.MemoryMapped,
		TestTraits.MemoryMapped | TestTraits.Merklized,
		TestTraits.MemoryMapped | TestTraits.PersistentIgnorant,
		TestTraits.MemoryMapped | TestTraits.Merklized | TestTraits.PersistentIgnorant
	];

	public static readonly IEnumerable<TestTraits> PersistentIgnorantTestCases = [
		TestTraits.PersistentIgnorant | TestTraits.FileMapped,
		TestTraits.PersistentIgnorant | TestTraits.MemoryMapped,
		TestTraits.PersistentIgnorant | TestTraits.FileMapped | TestTraits.Merklized,
		TestTraits.PersistentIgnorant | TestTraits.MemoryMapped | TestTraits.Merklized
	];

	
	public static readonly TestTraits[] MerkleizedTestCases = [
		TestTraits.MemoryMapped | TestTraits.Merklized,
		TestTraits.MemoryMapped | TestTraits.Merklized | TestTraits.PersistentIgnorant,
		TestTraits.FileMapped | TestTraits.Merklized,
		TestTraits.FileMapped | TestTraits.Merklized | TestTraits.PersistentIgnorant,
	];
}