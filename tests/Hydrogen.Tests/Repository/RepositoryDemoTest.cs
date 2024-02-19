//// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//// Author: Herman Schoenfeld
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using NUnit.Framework;

//namespace Hydrogen.Tests;

//[TestFixture, Timeout(60000)]
//public class RepositoryTests {

//	[SetUp]
//	public void InitTest() {
//		Assume.That(!Tools.NUnit.IsGitHubAction, "Test is disabled on GitHub Actions");
//	}

//	[Test]
//	public void Create() {
//		var repoPath = Tools.FileSystem.GetTempFileName(false);
//		var txnDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//		using var disposables = new Disposables();
//		disposables.Add(() => File.Delete(repoPath));
//		disposables.Add(() => Directory.Delete(txnDir, true));

//		using var repo = new PlayerRepository(repoPath, txnDir);
//		Assert.That(File.Exists(repoPath));
//		Assert.That(Directory.Exists(txnDir));
//		Assert.That(Tools.FileSystem.CountDirectoryContents(txnDir), Is.EqualTo(0));
//	}

//	[Test]
//	public void OpenEmpty() {
//		var repoPath = Tools.FileSystem.GetTempFileName(false);
//		var txnDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//		using var disposables = new Disposables();
//		disposables.Add(() => File.Delete(repoPath));
//		disposables.Add(() => Directory.Delete(txnDir, true));

//		using (new PlayerRepository(repoPath, txnDir)) {
//		}

//		IRepository<PlayerRecord, string> repo = null;
//		Assert.That(() => { repo = new PlayerRepository(repoPath, txnDir); }, Throws.Nothing);
//		disposables.Add(repo);
//	}


//	[Test]
//	public void Add() {
//		var repoPath = Tools.FileSystem.GetTempFileName(false);
//		var txnDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//		using var disposables = new Disposables();
//		disposables.Add(() => File.Delete(repoPath));
//		disposables.Add(() => Directory.Delete(txnDir, true));
//		var now = DateTime.Now;
//		using var repo = new PlayerRepository(repoPath, txnDir);
//		repo.Create(new PlayerRecord {
//			TID = "username",
//			PID = 123,
//			FirstSeen = now,
//			LastSeen = null
//		});
//		repo.Commit();
//		Assert.That(repo.Contains("username"), Is.True);
//		Assert.That(repo.TryGet("username", out var record), Is.True);
//		Assert.That(record.TID, Is.EqualTo("username"));
//		Assert.That(record.PID, Is.EqualTo(123));
//		Assert.That(record.FirstSeen, Is.EqualTo(now));
//		Assert.That(record.LastSeen, Is.Null);

//	}

//	[Test]
//	public void Add_ReopenConsistency_Commit() {
//		var repoPath = Tools.FileSystem.GetTempFileName(false);
//		var txnDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//		using var disposables = new Disposables();
//		disposables.Add(() => File.Delete(repoPath));
//		disposables.Add(() => Directory.Delete(txnDir, true));
//		var now = DateTime.Now;
//		using (var repo = new PlayerRepository(repoPath, txnDir)) {
//			repo.Create(new PlayerRecord {
//				TID = "username",
//				PID = 123,
//				FirstSeen = now,
//				LastSeen = null
//			});
//			repo.Commit();
//		}

//		// check file system integrity
//		Assert.That(File.Exists(repoPath));
//		Assert.That(Directory.Exists(txnDir));
//		Assert.That(Tools.FileSystem.CountDirectoryContents(txnDir), Is.EqualTo(0));

//		// check repository integrity
//		using (var repo = new PlayerRepository(repoPath, txnDir)) {
//			Assert.That(repo.Contains("username"), Is.True);
//			Assert.That(repo.TryGet("username", out var record), Is.True);
//			Assert.That(record.TID, Is.EqualTo("username"));
//			Assert.That(record.PID, Is.EqualTo(123));
//			Assert.That(record.FirstSeen, Is.EqualTo(now));
//			Assert.That(record.LastSeen, Is.Null);
//		}

//	}

//	[Test]
//	public void Add_ReopenConsistency_Rollback() {
//		var repoPath = Tools.FileSystem.GetTempFileName(false);
//		var txnDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//		using var disposables = new Disposables();
//		disposables.Add(() => File.Delete(repoPath));
//		disposables.Add(() => Directory.Delete(txnDir, true));
//		var now = DateTime.Now;
//		using (var repo = new PlayerRepository(repoPath, txnDir)) {
//			repo.Create(new PlayerRecord {
//				TID = "username",
//				PID = 123,
//				FirstSeen = now,
//				LastSeen = null
//			});
//			repo.Rollback();
//		}

//		// check file system integrity
//		Assert.That(File.Exists(repoPath));
//		Assert.That(Directory.Exists(txnDir));
//		Assert.That(Tools.FileSystem.CountDirectoryContents(txnDir), Is.EqualTo(0));

//		// check repository integrity
//		using (var repo = new PlayerRepository(repoPath, txnDir)) {
//			Assert.That(repo.Contains("username"), Is.False);
//		}

//	}

//	[Test]
//	public void Add_ReopenConsistency_UncommittedSameAsRollback() {
//		var repoPath = Tools.FileSystem.GetTempFileName(false);
//		var txnDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//		using var disposables = new Disposables();
//		disposables.Add(() => File.Delete(repoPath));
//		disposables.Add(() => Directory.Delete(txnDir, true));
//		var now = DateTime.Now;
//		using (var repo = new PlayerRepository(repoPath, txnDir)) {
//			repo.Create(new PlayerRecord {
//				TID = "username",
//				PID = 123,
//				FirstSeen = now,
//				LastSeen = null
//			});
//			// Do not Commit or Rollback (should be same as Rollback)
//		}

//		// check file system integrity
//		Assert.That(File.Exists(repoPath));
//		Assert.That(Directory.Exists(txnDir));
//		Assert.That(Tools.FileSystem.CountDirectoryContents(txnDir), Is.EqualTo(0));

//		// check repository integrity
//		using (var repo = new PlayerRepository(repoPath, txnDir)) {
//			Assert.That(repo.Contains("username"), Is.False);
//		}

//	}


//	[Test]
//	public async Task BattleTest_SyncOnly([Values(0, 1, 11, 111, 1111)] int records) {
//		var repoPath = Tools.FileSystem.GetTempFileName(false);
//		var txnDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//		using var disposables = new Disposables();
//		disposables.Add(() => File.Delete(repoPath));
//		disposables.Add(() => Directory.Delete(txnDir, true));
//		var now = DateTime.Now;
//		await using (var repo = new PlayerRepository(repoPath, txnDir)) {
//			repo.Create(new PlayerRecord {
//				TID = "username",
//				PID = 123,
//				FirstSeen = now,
//				LastSeen = null
//			});


//			var factory = new TaskFactory();

//			// create all even records
//			var createEvens = factory.StartNew(() => {
//				for (var i = 0; i < records; i++) {
//					// 0, 2, 4, ...
//					if (i % 2 == 1)
//						continue;

//					repo.Create(new PlayerRecord {
//						TID = $"username{i}",
//						PID = i,
//						FirstSeen = now,
//						LastSeen = null
//					});
//				}
//			});

//			// update all even records (and delete even records in 20's)
//			var updateEvens = factory.StartNew(() => {
//				for (var i = 0; i < records; i++) {
//					// 0, 2, 4, ...
//					if (i % 2 == 1)
//						continue;

//					while (!repo.Contains($"username{i}")) {
//						Thread.Sleep(1);
//					}

//					using (repo.EnterWriteScope()) {
//						if (i >= 20 && i <= 29) {
//							repo.Delete($"username{i}");
//							continue;
//						}
//						var item = repo.Get($"username{i}");
//						item.LastSeen = item.FirstSeen.Value.AddDays(i);
//						repo.Update(item);

//						var check = repo.Get($"username{i}");
//						Assert.That(check.LastSeen, Is.EqualTo(item.LastSeen));
//					}
//				}
//			});

//			// create odds
//			var createOdds = factory.StartNew(() => {
//				Parallel.For(0,
//					records,
//					i => {
//						// 1, 3, 5, ...
//						if (i % 2 == 0)
//							return;
//						repo.Create(new PlayerRecord {
//							TID = $"username{i}",
//							PID = i,
//							FirstSeen = now,
//							LastSeen = null
//						});
//					});
//			});

//			// update all odds records (and delete odds records in 20's)
//			var updateOdds = factory.StartNew(() => {
//				Parallel.For(0,
//					records,
//					i => {
//						// 1, 3, 5, ...
//						if (i % 2 == 0)
//							return;

//						while (!repo.Contains($"username{i}")) {
//							Thread.Sleep(1);
//						}

//						using (repo.EnterWriteScope()) {
//							if (i >= 20 && i <= 29) {
//								repo.Delete($"username{i}");
//								return;
//							}
//							var item = repo.Get($"username{i}");
//							item.LastSeen = item.FirstSeen.Value.AddDays(i);
//							repo.Update(item);
//						}
//					});
//			});

//			await Task.WhenAll(createEvens, updateEvens, createOdds, updateOdds);
//			repo.Commit();

//		}

//		// check file system integrity
//		Assert.That(File.Exists(repoPath));
//		Assert.That(Directory.Exists(txnDir));
//		Assert.That(Tools.FileSystem.CountDirectoryContents(txnDir), Is.EqualTo(0));

//		// check repository integrity
//		await using (var repo = new PlayerRepository(repoPath, txnDir)) {
//			for (var i = 0; i < records; i++) {
//				var contains = repo.Contains($"username{i}");
//				if (i >= 20 && i <= 29) {
//					Assert.That(contains, Is.False);
//					continue;
//				}

//				Assert.That(contains, Is.True);
//				var item = repo.Get($"username{i}");
//				Assert.That(item, Is.Not.Null);
//				Assert.That(item.TID, Is.EqualTo($"username{i}"));
//				Assert.That(item.PID, Is.EqualTo(i));
//				Assert.That(item.FirstSeen, Is.EqualTo(now));
//				Assert.That(item.LastSeen, Is.EqualTo(item.FirstSeen.Value.AddDays(i)));
//			}

//		}

//	}

//	[Test]
//	public async Task BattleTest_SyncAsyncMix([Values(0, 1, 11, 111, 1111)] int records) {
//		Assume.That(false, "This test is disabled, needs investigation");
//		var repoPath = Tools.FileSystem.GetTempFileName(false);
//		var txnDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//		using var disposables = new Disposables();
//		disposables.Add(() => File.Delete(repoPath));
//		disposables.Add(() => Directory.Delete(txnDir, true));
//		var now = DateTime.Now;
//		await using (var repo = new PlayerRepository(repoPath, txnDir)) {
//			await repo.CreateAsync(new PlayerRecord {
//				TID = "username",
//				PID = 123,
//				FirstSeen = now,
//				LastSeen = null
//			});


//			var factory = new TaskFactory();

//			// create all even records
//			var createEvens = factory.StartNew(() => {
//				for (var i = 0; i < records; i++) {
//					// 0, 2, 4, ...
//					if (i % 2 == 1)
//						continue;

//					repo.Create(new PlayerRecord {
//						TID = $"username{i}",
//						PID = i,
//						FirstSeen = now,
//						LastSeen = null
//					});
//				}
//			});


//			// update all even records (and delete even records in 20's)
//			var updateEvens = factory.StartNew(() => {
//				for (var i = 0; i < records; i++) {
//					// 0, 2, 4, ...
//					if (i % 2 == 1)
//						continue;

//					while (!repo.Contains($"username{i}")) {
//						Thread.Sleep(1);
//					}

//					using (repo.EnterWriteScope()) {
//						if (i >= 20 && i <= 29) {
//							repo.Delete($"username{i}");
//							continue;
//						}
//						var item = repo.Get($"username{i}");
//						item.LastSeen = item.FirstSeen.Value.AddDays(i);
//						repo.UpdateAsync(item).Wait();
//					}
//				}
//			});

//			// create odds
//			var createOdds = factory.StartNew(() => {
//				Parallel.For(0,
//					records,
//					i => {
//						// 1, 3, 5, ...
//						if (i % 2 == 0)
//							return;

//						repo.CreateAsync(new PlayerRecord {
//							TID = $"username{i}",
//							PID = i,
//							FirstSeen = now,
//							LastSeen = null
//						}).Wait();
//					});
//			});

//			// update all odds records (and delete odds records in 20's)
//			var updateOdds = factory.StartNew(() => {
//				Parallel.For(0,
//					records,
//					i => {
//						// 1, 3, 5, ...
//						if (i % 2 == 0)
//							return;

//						while (!repo.ContainsAsync($"username{i}").Result) {
//							Thread.Sleep(1);
//						}

//						using (repo.EnterWriteScope()) {
//							if (i >= 20 && i <= 29) {
//								repo.Delete($"username{i}");
//								return;
//							}
//							var item = repo.Get($"username{i}");
//							item.LastSeen = item.FirstSeen.Value.AddDays(i);
//							repo.UpdateAsync(item).Wait();
//						}
//					});
//			});


//			await Task.WhenAll(createEvens, updateEvens, createOdds, updateOdds);

//			await repo.CommitAsync();

//		}

//		// check file system integrity
//		Assert.That(File.Exists(repoPath));
//		Assert.That(Directory.Exists(txnDir));
//		Assert.That(Tools.FileSystem.CountDirectoryContents(txnDir), Is.EqualTo(0));

//		// check repository integrity
//		await using (var repo = new PlayerRepository(repoPath, txnDir)) {
//			for (var i = 0; i < records; i++) {
//				var contains = repo.Contains($"username{i}");
//				if (i >= 20 && i <= 29) {
//					Assert.That(contains, Is.False);
//					continue;
//				}

//				Assert.That(contains, Is.True);
//				var item = repo.Get($"username{i}");
//				Assert.That(item, Is.Not.Null);
//				Assert.That(item.TID, Is.EqualTo($"username{i}"));
//				Assert.That(item.PID, Is.EqualTo(i));
//				Assert.That(item.FirstSeen, Is.EqualTo(now));
//				Assert.That(item.LastSeen, Is.EqualTo(item.FirstSeen.Value.AddDays(i)));
//			}

//		}

//	}

//}


//public class PlayerRecord {

//	public string TID { get; set; }

//	public long PID { get; set; }

//	public DateTime? FirstSeen { get; set; }

//	public DateTime? LastSeen { get; set; }

//}


//public class PlayerRecordSerializer : ItemSerializerBase<PlayerRecord> {

//	private readonly StringSerializer _stringSerializer = new StringSerializer(Encoding.UTF8, SizeDescriptorStrategy.UseUInt32);
//	private readonly PrimitiveSerializer<long> _int64Serializer = new();
//	private readonly NullableSerializer<DateTime> _dateTimeSerializer = new(new DateTimeSerializer());

//	public override long CalculateSize(SerializationContext context, PlayerRecord item)
//		=> _stringSerializer.CalculateSize(item.TID) +
//		   _int64Serializer.CalculateSize(item.PID) +
//		   _dateTimeSerializer.CalculateSize(item.FirstSeen) +
//		   _dateTimeSerializer.CalculateSize(item.LastSeen);

//	public override void Serialize(PlayerRecord item, EndianBinaryWriter writer, SerializationContext context) {
//		_stringSerializer.Serialize(item.TID, writer);
//		_int64Serializer.Serialize(item.PID, writer);
//		_dateTimeSerializer.Serialize(item.FirstSeen, writer);
//		_dateTimeSerializer.Serialize(item.LastSeen, writer);
//	}

//	public override PlayerRecord Deserialize(EndianBinaryReader reader, SerializationContext context)
//		=> new PlayerRecord {
//			TID = _stringSerializer.Deserialize(reader),
//			PID = _int64Serializer.Deserialize(reader),
//			FirstSeen = _dateTimeSerializer.Deserialize(reader),
//			LastSeen = _dateTimeSerializer.Deserialize(reader),
//		};
//}


//public class PlayerRepository : IRepository<PlayerRecord, string>, ISynchronizedObject, ITransactionalObject {

//	public event EventHandlerEx<PlayerRecord>? Changing {
//		add => _repoAdapter.Changing += value;
//		remove => _repoAdapter.Changing -= value;
//	}

//	public event EventHandlerEx<PlayerRecord>? Changed {
//		add => _repoAdapter.Changed += value;
//		remove => _repoAdapter.Changed -= value;
//	}

//	public event EventHandlerEx<PlayerRecord>? Saving {
//		add => _repoAdapter.Saving += value;
//		remove => _repoAdapter.Saving -= value;
//	}

//	public event EventHandlerEx<PlayerRecord>? Saved {
//		add => _repoAdapter.Saved += value;
//		remove => _repoAdapter.Saved -= value;
//	}

//	public event EventHandlerEx? Clearing {
//		add => _repoAdapter.Clearing += value;
//		remove => _repoAdapter.Clearing -= value;
//	}

//	public event EventHandlerEx? Cleared {
//		add => _repoAdapter.Cleared += value;
//		remove => _repoAdapter.Cleared -= value;
//	}

//	public event EventHandlerEx<PlayerRecord>? Adding {
//		add => _repoAdapter.Adding += value;
//		remove => _repoAdapter.Adding -= value;
//	}

//	public event EventHandlerEx<PlayerRecord>? Added {
//		add => _repoAdapter.Changing += value;
//		remove => _repoAdapter.Changing -= value;
//	}

//	public event EventHandlerEx<PlayerRecord>? Updating {
//		add => _repoAdapter.Updating += value;
//		remove => _repoAdapter.Updating -= value;
//	}

//	public event EventHandlerEx<PlayerRecord>? Updated {
//		add => _repoAdapter.Updated += value;
//		remove => _repoAdapter.Updated -= value;
//	}

//	public event EventHandlerEx<object>? Committing {
//		add => _transactionalDict.Committing += value;
//		remove => _transactionalDict.Committing -= value;
//	}

//	public event EventHandlerEx<object>? Committed {
//		add => _transactionalDict.Committed += value;
//		remove => _transactionalDict.Committed -= value;
//	}

//	public event EventHandlerEx<object>? RollingBack {
//		add => _transactionalDict.RollingBack += value;
//		remove => _transactionalDict.RollingBack -= value;
//	}

//	public event EventHandlerEx<object>? RolledBack {
//		add => _transactionalDict.RolledBack += value;
//		remove => _transactionalDict.RolledBack -= value;
//	}

//	private readonly DictionaryRepositoryAdapter<PlayerRecord, string> _repoAdapter;
//	private readonly TransactionalDictionary<string, PlayerRecord> _transactionalDict;
//	private readonly SynchronizedObject _lock = new();

//	public PlayerRepository(string repoPath, string txnDir) {
//		if (!Path.Exists(txnDir))
//			Tools.FileSystem.CreateDirectory(txnDir);

//		_transactionalDict = new TransactionalDictionary<string, PlayerRecord>(
//			HydrogenFileDescriptor.From(repoPath, txnDir, containerPolicy: ClusteredStreamsPolicy.Default),
//			new StringSerializer(),
//			new PlayerRecordSerializer(),
//			keyChecksum: new ObjectHashCodeChecksummer<string>()
//		);
//		if (_transactionalDict.RequiresLoad)
//			_transactionalDict.Load();

//		_repoAdapter = new DictionaryRepositoryAdapter<PlayerRecord, string>(_transactionalDict, p => p.TID);

//	}

//	public ISynchronizedObject ParentSyncObject {
//		get => _lock.ParentSyncObject;
//		set => _lock.ParentSyncObject = value;
//	}

//	public ReaderWriterLockSlim ThreadLock => _lock.ParentSyncObject.ThreadLock;

//	public bool Contains(string identity) => _repoAdapter.Contains(identity);

//	public Task<bool> ContainsAsync(string identity) => _repoAdapter.ContainsAsync(identity);

//	public bool TryGet(string identity, out PlayerRecord entity) => _repoAdapter.TryGet(identity, out entity);

//	public Task<(bool, PlayerRecord)> TryGetAsync(string identity) => _repoAdapter.TryGetAsync(identity);

//	public void Create(PlayerRecord entity) => _repoAdapter.Create(entity);

//	public Task CreateAsync(PlayerRecord entity) => _repoAdapter.CreateAsync(entity);

//	public void Update(PlayerRecord entity) => _repoAdapter.Update(entity);

//	public Task UpdateAsync(PlayerRecord entity) => _repoAdapter.UpdateAsync(entity);

//	public void Delete(string identity) => _repoAdapter.Delete(identity);

//	public Task DeleteAsync(string identity) => _repoAdapter.DeleteAsync(identity);

//	public void Clear() => _repoAdapter.Clear();

//	public Task ClearAsync() => _repoAdapter.ClearAsync();

//	public PlayerRecord? Find(long ppid) {
//		using (EnterReadScope()) {
//			return _transactionalDict.Values.FirstOrDefault(p => p.PID == ppid);
//		}
//	}

//	public IDisposable EnterReadScope() => _lock.EnterReadScope();

//	public IDisposable EnterWriteScope() => _lock.EnterWriteScope();

//	public void Commit() => _transactionalDict.Commit();

//	public Task CommitAsync() => _transactionalDict.CommitAsync();

//	public void Rollback() => _transactionalDict.Rollback();

//	public Task RollbackAsync() => _transactionalDict.RollbackAsync();

//	public void Dispose() => _repoAdapter.Dispose();

//	public ValueTask DisposeAsync() => _repoAdapter.DisposeAsync();
//}
