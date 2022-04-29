//-----------------------------------------------------------------------
// <copyright file="CodeErrorsException.cs" company="Sphere 10 Software">
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
    
    /// <summary>
    /// Encapsulates a set of code errors captured during a compilation phase.
    /// </summary>
    public class CodeErrorsException : CodeErrorException {
        List<CodeErrorException> _errorList;

        public CodeErrorsException(List<CodeErrorException> errorList)
            : base(0,0,0,string.Empty) {
            _errorList = errorList;
        }

        public override int LineNumberOffset {
            get {
                if (_errorList.Count > 0) {
                    return _errorList[0].LineNumberOffset;
                }
                return 0;
            }
            set {
                foreach (CodeErrorException error in _errorList) {
                    error.LineNumberOffset = value;
                }
            }
        }

        public override int PositionOffset {
            get {
                if (_errorList.Count > 0) {
                    return _errorList[0].PositionOffset;
                }
                return 0;
            }
            set {
                foreach (CodeErrorException error in _errorList) {
                    error.PositionOffset = value;
                }
            }
        }

        public override string Message {
            get {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (CodeErrorException error in _errorList) {
                    stringBuilder.AppendLine(error.Message);
                }
                return stringBuilder.ToString();
            }
        }

    }
}
#endif
