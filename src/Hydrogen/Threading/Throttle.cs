// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;
using System.Threading;

namespace Hydrogen;

public sealed class Throttle : SynchronizedObject {
	public readonly TimeSpan _interval;
	public DateTime _lastEvent;
	private bool _waitingForStampede;

	public Throttle(float ratePerSecond)
		: this(TimeSpan.FromSeconds(1 / ratePerSecond)) {
	}

	public Throttle(TimeSpan interval) {
		_lastEvent = DateTime.MinValue;
		_interval = interval;
		_waitingForStampede = false;
	}


	public void Wait() {
		var now = DateTime.Now;
		var duration = now.Subtract(_lastEvent);
		if (duration < _interval) {
			Thread.Sleep(_interval.Subtract(duration).ClipTo(TimeSpan.Zero, TimeSpan.MaxValue));
		}
		_lastEvent = DateTime.Now;
	}

	public async Task WaitAsync() {
		var now = DateTime.Now;
		var duration = now.Subtract(_lastEvent);

		if (duration < _interval) {
			await Task.Delay(_interval.Subtract(duration).ClipTo(TimeSpan.Zero, TimeSpan.MaxValue));
		}

		_lastEvent = DateTime.Now;
	}

	public void RegisterEvent() {
		_lastEvent = DateTime.Now;
	}


	/// <summary>
	/// This throttle method returns true when interval from last call is at least the requested interval, false everything else.
	/// Useful when dealing with events that fire too often, and you only want to be notified once every period of time. Every 
	/// other notification be be scrapped.
	/// </summary>
	/// <returns><c>true</c> if this instance can pass; otherwise, <c>false</c>.</returns>
	public bool CanPass() {
		DateTime now = DateTime.Now;

		var canPass = true;
		using (EnterReadScope()) {
			canPass = now.Subtract(_lastEvent) >= _interval;
		}

		if (canPass) {
			using (EnterWriteScope()) {
				canPass = now.Subtract(_lastEvent) >= _interval;
				if (canPass) {
					_lastEvent = now;
					return true;
				}
			}
		}
		return false;
	}


	/// <summary>
	/// Waits the until a stampede of calls are finished. Two calls are considered in a "stampede" if they 
	/// occur within the interval provided in the constructor. This is useful if you are listening to a 
	/// collections remove event, but you only want to do something when all the removes have completed rather
	/// than doing something on every remove. The first caller in the stampede is kept waiting the other callers
	/// are dismissed.
	/// </summary>
	/// <returns>True - when the stampede has finished (first callers gets this), False - when the stampede is going on</returns>
	public async Task<bool> IsCallerFirstInStampede() {
		DateTime now = DateTime.Now;
		_lastEvent = now; // no need to lock for stampeding calls, faster this way, should be ok!

		using (EnterReadScope()) {
			if (_waitingForStampede)
				return false;
		}

		using (EnterWriteScope()) {
			if (_waitingForStampede)
				return false;
			_waitingForStampede = true;
		}

		while (_waitingForStampede) {
			await Task.Delay((int)_interval.TotalMilliseconds);

			using (EnterWriteScope()) {
				_waitingForStampede = DateTime.Now.Subtract(_lastEvent) < _interval;
			}
		}

		using (EnterWriteScope()) {
			_waitingForStampede = false;
		}
		return true;
	}

}
