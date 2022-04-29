//-----------------------------------------------------------------------
// <copyright file="Env.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Windows.LevelDB
{
    /// <summary>
    /// A default environment to access operating system functionality like 
    /// the filesystem etc of the current operating system.
    /// </summary>
    public class Env : LevelDBHandle
    {
        public Env()
        {
            Handle = LevelDBInterop.leveldb_create_default_env();
        }

        protected override void FreeUnManagedObjects()
        {
            LevelDBInterop.leveldb_env_destroy(Handle);
        }
    }
}
