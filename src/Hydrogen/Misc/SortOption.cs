//-----------------------------------------------------------------------
// <copyright file="SortOption.cs" company="Sphere 10 Software">
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

namespace Hydrogen {
	public class SortOption {

	    public SortOption(string name, SortDirection direction) {
	        Name = name;
	        Direction = direction;
	    }

        public string Name { get; set; }

        public SortDirection Direction { get; set; }
	}
}
