//-----------------------------------------------------------------------
// <copyright file="ApplicationChangedEvent.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework
{
	/// <summary>
	/// Summary description for ApplicationEvent.
	/// </summary>
	public class ApplicationChangedEvent : EventArgs 
	{
		public ApplicationChangedEvent(string targetAppName, string sourceApplicationName, DateTime switchedOn, TimeSpan openedFor) {
			TargetProcessName = targetAppName;
			SourceProcessName = sourceApplicationName;
			SwitchedOn = switchedOn;
			SourceOpenedFor = openedFor;
		}

		public string TargetProcessName {	get; private set; }
		public string SourceProcessName { get; private set; }
		public DateTime SwitchedOn { get; private set; }
		public TimeSpan SourceOpenedFor { get; private set; }
	}
}
