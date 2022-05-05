//-----------------------------------------------------------------------
// <copyright file="PerformanceCounter.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace SourceGrid.Utils
{
	public class PerformanceCounter : IDisposable, IPerformanceCounter
	{
		private DateTime m_start = DateTime.MinValue;
		
		public PerformanceCounter()
		{
			this.m_start = DateTime.Now;
		}
		
		public double GetSeconds()
		{
			TimeSpan span = DateTime.Now - m_start;
			return span.TotalSeconds;
		}
		
		public double GetMilisec()
		{
			TimeSpan span = DateTime.Now - m_start;
			return span.TotalMilliseconds;
		}
		
		public void Dispose()
		{
		}
	}
}
