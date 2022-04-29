//-----------------------------------------------------------------------
// <copyright file="CodeGenerationException.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Maths.Compiler {
    public class CodeGenerationException : CompilerException {

        public CodeGenerationException(string errMsg) 
            : base(errMsg) {
        }

        public override string Message {
            get {
                return string.Format("Code Generation Failed: {0}", base.Message);
            }
        }

    }
}
#endif
