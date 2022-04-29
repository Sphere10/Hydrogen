//-----------------------------------------------------------------------
// <copyright file="Token.cs" company="Sphere 10 Software">
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
    public class Token {
        private TokenType _type;
        private string _value;
        private int _line;
        private int _startPosition;
        private int _endPosition;

        public Token(TokenType type, string value, int line, int startPos, int endPos) {
            TokenType = type;
            Value = value;
            Line = line;
            StartPosition = startPos;
            EndPosition = endPos;
        }

        public string Value {
            get { return _value; }
            set { _value = value; }
        }
        public TokenType TokenType {
            get { return _type; }
            set { _type = value; }
        }
        public int Line {
            get { return _line; }
            set { _line = value; }
        }
        public int StartPosition {
            get { return _startPosition; }
            set { _startPosition = value; }
        }
        public int EndPosition {
            get { return _endPosition; }
            set { _endPosition = value; }
        }


    
    }
}
#endif
