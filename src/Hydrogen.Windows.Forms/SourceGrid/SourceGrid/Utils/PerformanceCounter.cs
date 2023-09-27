// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace SourceGrid.Utils;

public class PerformanceCounter : IDisposable, IPerformanceCounter {
	private DateTime m_start = DateTime.MinValue;

	public PerformanceCounter() {
		this.m_start = DateTime.Now;
	}

	public double GetSeconds() {
		TimeSpan span = DateTime.Now - m_start;
		return span.TotalSeconds;
	}

	public double GetMilisec() {
		TimeSpan span = DateTime.Now - m_start;
		return span.TotalMilliseconds;
	}

	public void Dispose() {
	}
}
