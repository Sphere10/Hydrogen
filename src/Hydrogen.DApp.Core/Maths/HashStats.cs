// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.DApp.Core.Maths;

public class PeriodicStatistics {

	private IList<Statistics> _history;
	private Statistics _currentPeriodStats;
	private TimeSpan _period;
	private bool _started;
	private DateTime _startedOn;

	public PeriodicStatistics(TimeSpan period, int historyLength)
		: this(period, historyLength, new List<Statistics>()) {
	}

	public PeriodicStatistics(TimeSpan period, int historyLength, IList<Statistics> store) {
		_history = store;
		_started = false;
	}

	public DateTime StartedOn {
		get {
			CheckStarted();
			return _startedOn;
		}
	}

	public int PeriodsAvailable {
		get {
			CheckStarted();
			return (int)Math.Ceiling((DateTime.UtcNow - _startedOn).TotalSeconds / _period.TotalSeconds);
		}
	}

	public void Start() {
		CheckNotStarted();
		_started = true;
	}

	public void RegisterEvent(double magnitude)
		=> RegisterEvent(magnitude, 1);

	public void RegisterEvent(double magnitude, int occurances) {
		CheckStarted();
		_currentPeriodStats.AddDatum(magnitude);
	}

	private void EnsurePeriodFresh() {
		var now = DateTime.UtcNow;
		var currentPeriodIndex = _history.Count;
		var currentPeriodStart = _startedOn + _period * currentPeriodIndex;
		var currentPeriodEnd = currentPeriodStart + _period;

		var nowIndex = (now - _startedOn);

	}

	private void CheckNotStarted() {
		Guard.Ensure(!_started, "Already started");
	}

	private void CheckStarted() {
		Guard.Ensure(_started, "Not started");
	}
}
