// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;

namespace Hydrogen;

public class SessionCache<K, T> : CacheBase<K, T> {
	private readonly Scheduler<IJob> _cleaner;

	public SessionCache(TimeSpan sessionTimeoutInterval)
		: base(CacheReapPolicy.None, ExpirationPolicy.SinceLastAccessedTime, expirationDuration: sessionTimeoutInterval) {
		_cleaner = new Scheduler<IJob>(SchedulerPolicy.DisposeWhenFinished | SchedulerPolicy.RemoveJobOnError | SchedulerPolicy.DontThrow);
		var job =
			JobBuilder
				.For(Cleanup)
				.RunAsyncronously()
				.RunOnce(DateTime.Now.Add(sessionTimeoutInterval))
				.Repeat
				.OnInterval(sessionTimeoutInterval)
				.Build();
		_cleaner.AddJob(job);
		_cleaner.Start();
	}

	public void KeepAlive(K sessionID) {
		base.Get(sessionID); // refresh cache
	}

	public virtual void Cleanup() {
		using (base.EnterWriteScope()) {
			var expired = base.InternalStorage.Where(kvp => IsExpired(kvp.Value)).ToArray();
			foreach (var item in expired) {
				base.Remove(item.Key);
			}
		}
	}

	public override void Dispose() {
		_cleaner.Stop();
		_cleaner.Dispose();
	}

	protected override long EstimateSize(T value) => 0;

	protected override T Fetch(K key) => throw new Exception($"Session '{key}' does not exist or has expired");

	protected override bool CheckStaleness(K key, CachedItem<T> item) => false;

}
