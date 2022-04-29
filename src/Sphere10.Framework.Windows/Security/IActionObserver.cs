//-----------------------------------------------------------------------
// <copyright file="IActionObserver.cs" company="Sphere 10 Software">
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
using System.Text;

namespace Sphere10.Framework.Windows.Security {
    
    public interface IActionObserver {

        void NotifyAction(string actionName, string objectType, string sourceName, string destName);

        void NotifyActionFailed(string action, string objectType, string sourceName, string destName, string reason);

        void NotifyInformation(string info, params object[] formatArgs);

        void NotifyWarning(string info, params object[] formatArgs);

        void NotifyError(string info, params object[] formatArgs);

        void NotifyCompleted();

    }
}
