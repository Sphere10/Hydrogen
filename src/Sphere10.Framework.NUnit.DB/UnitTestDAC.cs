//-----------------------------------------------------------------------
// <copyright file="UnitTestDAC.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Framework.Data;

namespace Sphere10.Framework.NUnit {
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
