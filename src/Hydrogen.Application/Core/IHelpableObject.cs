//-----------------------------------------------------------------------
// <copyright file="IHelpableObject.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Application {
	public interface IHelpableObject {

		HelpType Type { get; }

		string FileName { get; }

		string Url { get; } 

		int? PageNumber { get;}

		int? HelpTopicID { get; }

		int? HelpTopicAlias { get; }
	}
}