// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hydrogen.Windows.LevelDB;
using NUnit.Framework.Legacy;


// ReSharper disable CheckNamespace

namespace Hydrogen.UnitTests;

[TestFixture]
public class LevelDBTests {
	static string testPath;
	static string CleanTestDB() {
		testPath = Path.GetTempPath();
		DB.Destroy(testPath, new Options { CreateIfMissing = true });
		return testPath;
	}


	[Test]
	public void Intro() {
		using (var database = new DB("mytestdb", new Options() { CreateIfMissing = true })) {
			database.Put("key1", "value1");
			ClassicAssert.AreEqual("value1", database.Get("key1"));
			ClassicAssert.IsTrue(database.Get("key1") != null);
			database.Delete("key1");
			ClassicAssert.IsFalse(database.Get("key1") != null);
			ClassicAssert.IsNull(database.Get("key1"));
		}
	}

	[Test]
	public void TestOpen() {
		Assert.That(() => {
				var path = CleanTestDB();
				using (var db = new DB(path, new Options { CreateIfMissing = true })) {
				}

				using (var db = new DB(path, new Options { ErrorIfExists = true })) {
				}
			},
			Throws.TypeOf<LevelDBException>()
		);
	}

	[Test]
	public void TestCRUD() {
		var path = CleanTestDB();

		using (var db = new DB(path, new Options { CreateIfMissing = true })) {
			db.Put("Tampa", "green");
			db.Put("London", "red");
			db.Put("New York", "blue");

			ClassicAssert.AreEqual(db.Get("Tampa"), "green");
			ClassicAssert.AreEqual(db.Get("London"), "red");
			ClassicAssert.AreEqual(db.Get("New York"), "blue");

			db.Delete("New York");

			ClassicAssert.IsNull(db.Get("New York"));

			db.Delete("New York");
		}
	}

	[Test]
	public void TestRepair() {
		TestCRUD();
		DB.Repair(testPath, new Options());
	}

	[Test]
	public void TestIterator() {
		var path = CleanTestDB();

		using (var db = new DB(path, new Options { CreateIfMissing = true })) {
			db.Put("Tampa", "green");
			db.Put("London", "red");
			db.Put("New York", "blue");

			var expected = new[] { "London", "New York", "Tampa" };

			var actual = new List<string>();
			using (var iterator = db.CreateIterator(new ReadOptions())) {
				iterator.SeekToFirst();
				while (iterator.IsValid()) {
					var key = iterator.GetStringKey();
					actual.Add(key);
					iterator.Next();
				}
			}

			ClassicAssert.AreEqual(expected, actual);

		}
	}

	[Test]
	public void TestEnumerable() {
		var path = CleanTestDB();

		using (var db = new DB(path, new Options { CreateIfMissing = true })) {
			db.Put("Tampa", "green");
			db.Put("London", "red");
			db.Put("New York", "blue");

			var expected = new[] { "London", "New York", "Tampa" };
			var actual = from kv in db as IEnumerable<KeyValuePair<string, string>>
			             select kv.Key;

			ClassicAssert.AreEqual(expected, actual.ToArray());
		}
	}

	[Test]
	public void TestSnapshot() {
		var path = CleanTestDB();

		using (var db = new DB(path, new Options { CreateIfMissing = true })) {
			db.Put("Tampa", "green");
			db.Put("London", "red");
			db.Delete("New York");

			using (var snapShot = db.CreateSnapshot()) {
				var readOptions = new ReadOptions { Snapshot = snapShot };

				db.Put("New York", "blue");

				ClassicAssert.AreEqual(db.Get("Tampa", readOptions), "green");
				ClassicAssert.AreEqual(db.Get("London", readOptions), "red");

				// Snapshot taken before key was updates
				ClassicAssert.IsNull(db.Get("New York", readOptions));
			}

			// can see the change now
			ClassicAssert.AreEqual(db.Get("New York"), "blue");

		}
	}

	[Test]
	public void TestGetProperty() {
		var path = CleanTestDB();

		using (var db = new DB(path, new Options { CreateIfMissing = true })) {
			var r = new Random(0);
			var data = "";
			for (var i = 0; i < 1024; i++) {
				data += 'a' + r.Next(26);
			}

			for (int i = 0; i < 5 * 1024; i++) {
				db.Put(string.Format("row{0}", i), data);
			}

			var stats = db.PropertyValue("leveldb.stats");

			ClassicAssert.IsNotNull(stats);
			ClassicAssert.IsTrue(stats.Contains("Compactions"));
		}
	}

	[Test]
	public void TestWriteBatch() {
		var path = CleanTestDB();

		using (var db = new DB(path, new Options { CreateIfMissing = true })) {
			db.Put("NA", "Na");

			using (var batch = new WriteBatch()) {
				batch.Delete("NA")
					.Put("Tampa", "Green")
					.Put("London", "red")
					.Put("New York", "blue");
				db.Write(batch);
			}

			var expected = new[] { "London", "New York", "Tampa" };
			var actual = from kv in db as IEnumerable<KeyValuePair<string, string>>
			             select kv.Key;

			ClassicAssert.AreEqual(expected, actual.ToArray());
		}
	}
}
