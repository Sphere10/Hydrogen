// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Data;

namespace Hydrogen.NUnit {
	public class UnitTestDAC : DACDecorator, IDisposable {

		public UnitTestDAC(Action endAction, IDAC innerDAC) : base(innerDAC) {
			EndAction = endAction;
		}

		public Action EndAction { get; private set; }

		public void Dispose() {
			if (EndAction != null) {
				EndAction();
			}
		}
	}
}
