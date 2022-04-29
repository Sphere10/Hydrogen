//-----------------------------------------------------------------------
// <copyright file="ScannerState.cs" company="Sphere 10 Software">
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

    public enum ScannerState {
        Start,
        Identifier,
        Number,
        AlmostDecimal,
        Decimal,
        NumberWithExponent,
        NumberWithSignedExponent,
        NumberWithUnsignedExponent,
        Power,
        Plus,
        Minus,
        Multiplication,
        Division,
        OpenBracket,
        CloseBracket,
        OpenParenthesis,
        CloseParenthesis,
        BeginBracket,
        EndBracket,
        Assign,
        Equality,
        AlmostOr,
        Or,
        AlmostAnd,
        And,
        Comma,
        Dot,
        SemiColon,
        Not,
        Inequality,
        Modulus,
        LessThan,
        LessThanEqualTo,
        GreaterThan,
        GreaterThanEqualTo,
        EndOfCode,
    }
}
#endif
