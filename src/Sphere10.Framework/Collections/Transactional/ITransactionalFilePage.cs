//-----------------------------------------------------------------------
// <copyright file="PageManager.cs" company="Sphere 10 Software">
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

using System.IO;

namespace Sphere10.Framework {
	public interface ITransactionalFilePage<TItem> : IFilePage<TItem> {
		string UncommittedPageFileName { get; }

		bool HasUncommittedData { get; set; }

		Stream OpenSourceReadStream();

		Stream OpenSourceWriteStream();
	}


}