//-----------------------------------------------------------------------
// <copyright file="AlwaysOnScope.cs" company="Sphere 10 Software">
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
using Sphere10.Framework;

namespace Sphere10.Framework.Windows {
    public class AlwaysOnScope : IDisposable {
        private readonly uint _priorState ;
        public AlwaysOnScope(bool system,  bool display) {
            var mode = (uint)WinAPI.KERNEL32.ES_CONTINUOUS;
            if (system)
                mode = mode | WinAPI.KERNEL32.ES_SYSTEM_REQUIRED;
            if (display)
                mode = mode | WinAPI.KERNEL32.ES_DISPLAY_REQUIRED;
           _priorState = WinAPI.KERNEL32.SetThreadExecutionState(mode);

        }

        public void Dispose() {
            WinAPI.KERNEL32.SetThreadExecutionState(_priorState);
        }
    }
}
