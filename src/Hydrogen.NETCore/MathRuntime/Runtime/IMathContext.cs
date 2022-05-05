//-----------------------------------------------------------------------
// <copyright file="IMathContext.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Maths.Compiler {

    public interface IMathContext {
        IMathContext ParentContext { get; set; }

        IVariableContext Variables { get; }

        IFunctionContext Functions { get; }

        IFunction GenerateFunction(string expression);

        IFunction GenerateFunction(string expression, string parameterName);

    }

}
#endif
