//-----------------------------------------------------------------------
// <copyright file="CompilerException.cs" company="Sphere 10 Software">
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

#if !__MOBILE__
using System;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework.Maths.Compiler {
    

    /// <summary>
    /// Base exception class for all compiler generated exceptions.
    /// </summary>
    public class CompilerException : ApplicationException {

        public CompilerException(string errMsg) 
            : this(errMsg, null) {
        }

        public CompilerException(string errMsg, Exception innerException)
            : base (errMsg, innerException) {
        }
    }
}
#endif
