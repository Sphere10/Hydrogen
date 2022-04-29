//-----------------------------------------------------------------------
// <copyright file="AssemblyDefaultProductKeyAttribute.cs" company="Sphere 10 Software">
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

using System.Runtime.InteropServices;
using System;

namespace Sphere10.Framework.Application {

	[ComVisible(true)]
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class AssemblyDefaultProductKeyAttribute : Attribute {

        private string _key = string.Empty;

        public AssemblyDefaultProductKeyAttribute(string key) {
            ProductKey = key;
        }

        public string ProductKey {
            get {
                return _key;
            }
            set {
                _key = value;
            }
        }
    }

}
