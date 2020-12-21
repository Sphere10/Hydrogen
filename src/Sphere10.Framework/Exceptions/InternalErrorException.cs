//-----------------------------------------------------------------------
// <copyright file="InternalErrorException.cs" company="Sphere 10 Software">
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
using System.Threading;
using System.Diagnostics;

namespace Sphere10.Framework {
    public class InternalErrorException : SoftwareException {


        public InternalErrorException() 
         : this("An unexpected error has occured") {
        }

        public InternalErrorException(string error)
            : this(error, "0") {
        }

        public InternalErrorException(string error, params object[] formatArgs)
            : this(null, error, formatArgs) {
        }
        
        public InternalErrorException(Exception innerException, string error, params object[] formatArgs)
            : this(string.Format(error, formatArgs), innerException) {
        }

        public InternalErrorException(string error, Exception innerException)
            : base(error, innerException) {            
        }
    }
}
