//-----------------------------------------------------------------------
// <copyright file="SyntaxTree.cs" company="Sphere 10 Software">
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
using System.Diagnostics;

namespace Sphere10.Framework.Maths.Compiler {

    public class SyntaxTree {
        Token _token;

        public SyntaxTree(Token token) {
            Token = token;
        }

        public SyntaxTree() {
        }

        public Token Token {
            get { return _token; }
            set { _token = value; }
        }

    }
}
#endif
