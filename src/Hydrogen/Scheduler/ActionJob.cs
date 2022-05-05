//-----------------------------------------------------------------------
// <copyright file="ActionJob.cs" company="Sphere 10 Software">
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
using Hydrogen;

namespace Hydrogen {
	public class ActionJob : BaseJob  {
		private readonly Action _action;

		public ActionJob(Action action, string name = null) {
			_action = action;
			Name = name ?? $"Action Job {Guid.NewGuid().ToStrictAlphaString()}";
		}

		public override void Execute() {
			_action();
		}

        public override JobSerializableSurrogate ToSerializableSurrogate() {
			throw new NotSupportedException("Cannot serialize action jobs");
		}

		public override void FromSerializableSurrogate(JobSerializableSurrogate jobSurrogate)
		{
			throw new NotSupportedException("Cannot deserialize action jobs");
		}
	}
}
