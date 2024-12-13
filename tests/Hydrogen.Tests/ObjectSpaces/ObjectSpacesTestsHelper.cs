using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydrogen.ObjectSpaces;

namespace Hydrogen.Tests;

public static class ObjectSpaceTestsHelper {


	public static ObjectSpaceBase CreateStandard(ObjectSpaceTestTraits traits, Dictionary<string, object> activationArgs = default, IndexNullPolicy nullPolicy = IndexNullPolicy.IgnoreNull) {
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
		if (traits.HasFlag(ObjectSpaceTestTraits.PersistentIgnorant))
			builder.AutoSave();

		// memory mapped
		if (traits.HasFlag(ObjectSpaceTestTraits.MemoryMapped)) { 
			if (!activationArgs.TryGetValue("stream", out var stream)) {
				stream = new MemoryStream();
				disposables.Add((MemoryStream)stream);
			}
			builder.UseMemoryStream((MemoryStream)stream);
			 
		}

		// file mapped
		if (traits.HasFlag(ObjectSpaceTestTraits.FileMapped)) {
			if (!activationArgs.TryGetValue("folder", out var folder)) {
				folder = Tools.FileSystem.GetTempEmptyDirectory(true);
				disposables.Add(Tools.Scope.DeleteDirOnDispose((string)folder));
			}
			var file = Path.Combine((string)folder, "app.db");
			builder.UseFile(file);
		}

		// merkleized
		if (traits.HasFlag(ObjectSpaceTestTraits.Merklized))
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



	public static readonly IEnumerable<ObjectSpaceTestTraits> AllTestCases = [
		// FileMapped combinations
		ObjectSpaceTestTraits.FileMapped,
		ObjectSpaceTestTraits.FileMapped | ObjectSpaceTestTraits.Merklized,
		ObjectSpaceTestTraits.FileMapped | ObjectSpaceTestTraits.PersistentIgnorant,
		ObjectSpaceTestTraits.FileMapped | ObjectSpaceTestTraits.Merklized | ObjectSpaceTestTraits.PersistentIgnorant,

		// MemoryMapped combinations
		ObjectSpaceTestTraits.MemoryMapped,
		ObjectSpaceTestTraits.MemoryMapped | ObjectSpaceTestTraits.Merklized,
		ObjectSpaceTestTraits.MemoryMapped | ObjectSpaceTestTraits.PersistentIgnorant,
		ObjectSpaceTestTraits.MemoryMapped | ObjectSpaceTestTraits.Merklized | ObjectSpaceTestTraits.PersistentIgnorant
	];

	public static readonly ObjectSpaceTestTraits[] FileMappedTestCases = {
		ObjectSpaceTestTraits.FileMapped,
		ObjectSpaceTestTraits.FileMapped | ObjectSpaceTestTraits.Merklized,
		ObjectSpaceTestTraits.FileMapped | ObjectSpaceTestTraits.PersistentIgnorant,
		ObjectSpaceTestTraits.FileMapped | ObjectSpaceTestTraits.Merklized | ObjectSpaceTestTraits.PersistentIgnorant
	};

	public static readonly IEnumerable<ObjectSpaceTestTraits> MemoryMappedTestCases = [
		ObjectSpaceTestTraits.MemoryMapped,
		ObjectSpaceTestTraits.MemoryMapped | ObjectSpaceTestTraits.Merklized,
		ObjectSpaceTestTraits.MemoryMapped | ObjectSpaceTestTraits.PersistentIgnorant,
		ObjectSpaceTestTraits.MemoryMapped | ObjectSpaceTestTraits.Merklized | ObjectSpaceTestTraits.PersistentIgnorant
	];

	public static readonly IEnumerable<ObjectSpaceTestTraits> PersistentIgnorantTestCases = [
		ObjectSpaceTestTraits.PersistentIgnorant | ObjectSpaceTestTraits.FileMapped,
		ObjectSpaceTestTraits.PersistentIgnorant | ObjectSpaceTestTraits.MemoryMapped,
		ObjectSpaceTestTraits.PersistentIgnorant | ObjectSpaceTestTraits.FileMapped | ObjectSpaceTestTraits.Merklized,
		ObjectSpaceTestTraits.PersistentIgnorant | ObjectSpaceTestTraits.MemoryMapped | ObjectSpaceTestTraits.Merklized
	];

	
	public static readonly ObjectSpaceTestTraits[] MerkleizedTestCases = [
		ObjectSpaceTestTraits.MemoryMapped | ObjectSpaceTestTraits.Merklized,
		ObjectSpaceTestTraits.MemoryMapped | ObjectSpaceTestTraits.Merklized | ObjectSpaceTestTraits.PersistentIgnorant,
		ObjectSpaceTestTraits.FileMapped | ObjectSpaceTestTraits.Merklized,
		ObjectSpaceTestTraits.FileMapped | ObjectSpaceTestTraits.Merklized | ObjectSpaceTestTraits.PersistentIgnorant,
	];
}