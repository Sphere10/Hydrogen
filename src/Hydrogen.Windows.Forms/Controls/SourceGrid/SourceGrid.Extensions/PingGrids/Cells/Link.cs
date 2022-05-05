//-----------------------------------------------------------------------
// <copyright file="Link.cs" company="Sphere 10 Software">
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

namespace SourceGrid.Extensions.PingGrids.Cells
{
	public class Link : SourceGrid.Cells.Virtual.Link
	{
	        public Link()
		{
	            Model.AddModel(new PingGridValueModel());
		}
	}
}
