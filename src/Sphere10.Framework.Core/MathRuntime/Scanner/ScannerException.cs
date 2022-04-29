//-----------------------------------------------------------------------
// <copyright file="ScannerException.cs" company="Sphere 10 Software">
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
    public class ScannerException : CodeErrorException {

        public ScannerException(ScannerErrorState error, int line, int startPos, int endPos)
            : base(line, startPos, endPos, ConstructErrorMessage(error)) {
        }

        private static string ConstructErrorMessage(ScannerErrorState error) {
            string errMsg = string.Empty;
            switch (error) {
                case ScannerErrorState.CannotHaveBlankExponent:
                    errMsg = "Cannot have blank exponent";
                    break;
                case ScannerErrorState.CannotHaveBlankMantissa:
                    errMsg = "Cannot have blank mantissa";
                    break;
                case ScannerErrorState.InternalErrorCouldNotResolveTokenType:
                    errMsg = "Internal error could not resolve token type";
                    break;
                case ScannerErrorState.InternalErrorCouldNotTransitionState:
                    errMsg = "Internal error could not transition scanner state";
                    break;
                case ScannerErrorState.UnexpectedCharacter:
                    errMsg = "Unexpected character";
                    break;
                case ScannerErrorState.UnexpectedSymbol:
                    errMsg = "Unexpected symbol";
                    break;
            }
            return errMsg;
        }

    }
}
#endif
