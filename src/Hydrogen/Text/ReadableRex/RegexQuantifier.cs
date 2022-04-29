//-----------------------------------------------------------------------
// <copyright file="RegexQuantifier.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework {
	public class RegexQuantifier {
        readonly RegexPattern _quantifiedExpression;

        internal RegexQuantifier(RegexPattern quantifiedExpression) {
            _quantifiedExpression = quantifiedExpression;
        }

        public virtual RegexPattern Exactly(int timesToRepeat) {
            _quantifiedExpression.RegEx("{" + timesToRepeat + "}");
            return _quantifiedExpression;
        }

        public virtual RegexPattern ZeroOrMore {
            get {
                _quantifiedExpression.RegEx("*");
                return _quantifiedExpression;
            }
        }

        public virtual RegexPattern OneOrMore {
            get {
                _quantifiedExpression.RegEx("+");
                return _quantifiedExpression;
            }
        }

        public virtual RegexPattern Optional {
            get {
                _quantifiedExpression.RegEx("?");
                return _quantifiedExpression;
            }
        }

        public virtual RegexPattern AtLeast(int timesToRepeat) {
            _quantifiedExpression.RegEx("{" + timesToRepeat + ",}");
            return _quantifiedExpression;
        }

        public virtual RegexPattern AtMost(int timesToRepeat) {
            _quantifiedExpression.RegEx("{," + timesToRepeat + "}");
            return _quantifiedExpression;
        }

        public virtual RegexPattern InRange(int minimum, int maximum) {
            _quantifiedExpression.RegEx("{" + minimum + "," + maximum + "}");
            return _quantifiedExpression;
        }

        public RegexQuantifier Lazy => new RegexLazyQuantifier(_quantifiedExpression);
    }
}
