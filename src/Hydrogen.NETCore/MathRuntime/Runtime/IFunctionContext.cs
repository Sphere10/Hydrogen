//-----------------------------------------------------------------------
// <copyright file="IFunctionContext.cs" company="Sphere 10 Software">
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

    public interface IFunctionContext {
        IMathContext MathContext { get; }
        bool ContainsFunction(string name);
        void RemoveFunction(string name);
        IFunction this[string var] { get; set; }
    }

}
#endif
