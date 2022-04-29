//-----------------------------------------------------------------------
// <copyright file="BinaryOperatorTree.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Maths.Compiler
{
    public class BinaryOperatorTree : SyntaxTree
    {
        private SyntaxTree _leftHandSide;
        private SyntaxTree _rightHandSide;
        private Operator _operator;



        public BinaryOperatorTree(Token token)
            : base(token)
        {
        }

        public Operator Operator
        {
            get { return _operator; }
            set { _operator = value; }
        }

        public SyntaxTree LeftHandSide
        {
            get { return _leftHandSide; }
            set { _leftHandSide = value; }
        }

        public SyntaxTree RightHandSide
        {
            get { return _rightHandSide; }
            set { _rightHandSide = value; }
        }

        public override string ToString()
        {
            Debug.Assert(_leftHandSide != null);
            Debug.Assert(_rightHandSide != null);
            return string.Format("{0}({1},{2})", Operator, LeftHandSide.ToString(), RightHandSide.ToString());
        }
    }
}
#endif
