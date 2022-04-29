//-----------------------------------------------------------------------
// <copyright file="FactorTree.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Maths.Compiler
{


    public class FactorTree : SyntaxTree
    {

        public FactorTree()
        {
        }

        public FactorTree(Token token)
            : base(token)
        {
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", Token.TokenType, Token.Value);
        }
    }

}
#endif
