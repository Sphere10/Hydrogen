//-----------------------------------------------------------------------
// <copyright file="SQLExpression.cs" company="Sphere 10 Software">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrogen.Data {

    public class SQLExpression  {
        
        internal SQLExpression(SQLOperator op, IEnumerable<SQLExpression> expressions) {
            Type = SQLExpressionType.Node;
            Operator = op;
            Expressions = expressions;
            ValueKind = SQLExpressionValueType.None;
            Value = null;
        }

        internal SQLExpression(SQLExpressionValueType valueKind, object value) {
            Type = SQLExpressionType.Leaf;
            ValueKind = valueKind;
            Value = value;
        }

        public readonly SQLExpressionType Type;
        public readonly SQLOperator Operator;
        public readonly IEnumerable<SQLExpression> Expressions;
        public readonly SQLExpressionValueType ValueKind;
        public readonly object Value;


    }


    
}
