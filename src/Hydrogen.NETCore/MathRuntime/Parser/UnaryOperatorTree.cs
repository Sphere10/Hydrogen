//-----------------------------------------------------------------------
// <copyright file="UnaryOperatorTree.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Maths.Compiler
{
    public class UnaryOperatorTree : SyntaxTree
    {
        private SyntaxTree _operand;
        private Operator _operator;



        public UnaryOperatorTree(Token token)
            : base(token)
        {
        }

        public SyntaxTree Operand
        {
            get { return _operand; }
            set { _operand = value; }
        }

        public Operator Operator
        {
            get { return _operator; }
            set { _operator = value; }

        }
        public override string ToString()
        {
            Debug.Assert(_operand != null);
            return string.Format("{0}({1})", Operator, Operand.ToString());
        }
    }
}
#endif
