//-----------------------------------------------------------------------
// <copyright file="LevelDBException.cs" company="Sphere 10 Software">
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
using System.Runtime.InteropServices;
using System.Text;

namespace Hydrogen.Windows.LevelDB
{
    public class LevelDBException : Exception
    {
        public LevelDBException(string message) : base(message) { }

        public static void Check(IntPtr error)
        {
            if (error != IntPtr.Zero)
            {
                try
                {
                    var message = Marshal.PtrToStringAnsi(error);
                    throw new LevelDBException(message);
                }
                finally
                {
                    LevelDBInterop.leveldb_free(error);
                }
            }
        }
    }
}
