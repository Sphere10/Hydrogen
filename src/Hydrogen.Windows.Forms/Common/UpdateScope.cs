//-----------------------------------------------------------------------
// <copyright file="UpdateScope.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Windows.Forms {

    internal class UpdateScope : IDisposable {
	    private readonly FinishedUpdateBehaviour _behaviour;
	    private readonly IUpdatable _updateable;

        public UpdateScope(IUpdatable updateable, FinishedUpdateBehaviour behaviour) {
            if (!updateable.Updating) {
                _updateable = updateable;
                _updateable.BeginUpdate();
                _behaviour = behaviour;
            }
        }

        public void Dispose() {
            if (_updateable != null) {
                _updateable.FinishUpdate(_behaviour);
            }
        }
    }

}
