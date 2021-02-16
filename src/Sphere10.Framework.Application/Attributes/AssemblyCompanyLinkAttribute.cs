//-----------------------------------------------------------------------
// <copyright file="AssemblyCompanyLinkAttribute.cs" company="Sphere 10 Software">
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
using System.Text;
using System.Runtime.InteropServices;

namespace Sphere10.Framework.Application {

    [ComVisible(true)]
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class AssemblyCompanyLinkAttribute : Attribute {
        private string _companyLink = null;
    
        public AssemblyCompanyLinkAttribute(string url) {
            CompanyLink = url;
        }

        public string CompanyLink {
            get {
                return _companyLink;
            }
            set {
                _companyLink = value;
            }
        }
    }
}

