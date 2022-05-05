//-----------------------------------------------------------------------
// <copyright file="AssemblyProductLinkAttribute.cs" company="Sphere 10 Software">
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
using System.Runtime.InteropServices;

namespace Hydrogen.Application {

    [ComVisible(true)]
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class AssemblyProductLinkAttribute : Attribute {
        public AssemblyProductLinkAttribute(string url) {
            ProductLink = url;
        }

        public string ProductLink { get; set; }
    }

}


