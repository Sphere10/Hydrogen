//-----------------------------------------------------------------------
// <copyright file="DefaultMathContext.cs" company="Sphere 10 Software">
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

    public class DefaultMathContext : IMathContext {

        private IMathContext _parentContext;
        private IVariableContext _variableContext;
        private IFunctionContext _functionContext;

        public DefaultMathContext() {
            _parentContext = null;
            _variableContext = new DefaultVariableContext(this);
            _functionContext = new DefaultFunctionContext(this);
        }

        public IMathContext ParentContext {
            get {
                return _parentContext;
            }
            set {
                _parentContext = value;
            }
        }

        public IVariableContext Variables {
            get {
                return _variableContext;
            }
        }

        public IFunctionContext Functions {
            get {
                return _functionContext;
            }
        }

        public IFunction GenerateFunction(string expression) {
            return GenerateFunction(expression, "x");
        }

        public IFunction GenerateFunction(string expression, string parameterName)
        {
            BasicFunctionGenerator functionGenerator = new BasicFunctionGenerator(this);
            functionGenerator.FunctionParameterName = parameterName;
            return functionGenerator.GenerateFunctionFromExpression(expression);
        }

    }
}
#endif
