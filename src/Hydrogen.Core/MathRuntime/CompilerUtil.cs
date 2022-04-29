//-----------------------------------------------------------------------
// <copyright file="CompilerUtil.cs" company="Sphere 10 Software">
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
    public class CompilerUtil {

        public static bool IsValidVariable(string var) {
            bool retval = false;
            if (var.Length > 0) {
                // first character must be letter
                if (char.IsLetter(var[0])) {
                    // all remaining characters must be letters or digits
                    retval = true;
                    foreach (char c in var.Substring(1).ToCharArray()) {
                        if (!char.IsLetterOrDigit(c)) {
                            retval = false;
                            break;
                        }
                    }
                }
            }
            return retval;
        }


    }
}
#endif
