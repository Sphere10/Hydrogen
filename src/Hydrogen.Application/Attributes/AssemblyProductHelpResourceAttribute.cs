//-----------------------------------------------------------------------
// <copyright file="AssemblyProductHelpResourceAttribute.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Application {

	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public abstract class AssemblyProductHelpResourceAttribute : Attribute {

        public AssemblyProductHelpResourceAttribute(HelpType helpType, string path)
        {
            HelpType = helpType;
            Path = path;
        }

        public HelpType HelpType { get; set; }

    	public string Path { get; set; }

        
    }

}


