// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Maths {

	public class FunctionIntegral {
        private IFunction _function = null;
        private double _from;

        public FunctionIntegral(IFunction function, double fromParam) {
            _function = function;
            _from = fromParam;
        }

        public double Eval(double x) {
            return Tools.MathPlus.Integrate(_function, _from, x);
        }


    }
}