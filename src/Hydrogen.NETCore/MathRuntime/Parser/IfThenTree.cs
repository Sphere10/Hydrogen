//-----------------------------------------------------------------------
// <copyright file="IfThenTree.cs" company="Sphere 10 Software">
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

    public class IfThenTree : SyntaxTree
    {
        private SyntaxTree _condition;
        private SyntaxTree _expression;

        public IfThenTree()
            : base()
        {
        }

        public SyntaxTree Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public SyntaxTree Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }

        public override string ToString()
        {
            return string.Format("IFTHEN({0},{1})", Condition, Expression);
        }
    }
}
#endif
