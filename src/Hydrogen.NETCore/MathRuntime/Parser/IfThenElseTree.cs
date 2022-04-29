//-----------------------------------------------------------------------
// <copyright file="IfThenElseTree.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Maths.Compiler
{
    public class IfThenElseTree : IfThenTree
    {
        private SyntaxTree _elseExpression;

        public IfThenElseTree()
            : base()
        {
        }

        public SyntaxTree ElseExpression
        {
            get { return _elseExpression; }
            set { _elseExpression = value; }
        }

        public override string ToString()
        {
            return string.Format(
                "IFTHENELSE({0},{1},{2})",
                Condition.ToString(),
                Expression.ToString(),
                ElseExpression.ToString()
            );
        }
    }
}
#endif
