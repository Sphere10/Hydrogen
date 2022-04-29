//-----------------------------------------------------------------------
// <copyright file="NTLocalObject.cs" company="Sphere 10 Software">
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
using System.Security.Principal;


namespace Sphere10.Framework.Windows.Security {

    /// <summary>
    /// Represents a local object like a local user or local group. 
    /// </summary>
    public abstract class NTLocalObject : NTObject {
        private string _description;

        public string Description {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Updates local state of object to host.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Refreshes state from host.
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        /// Deletes this object from host.
        /// </summary>
        public abstract void Delete();

    }

}
