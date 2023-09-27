//-----------------------------------------------------------------------
// <copyright file="ApplicationInitializerBase.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

namespace Hydrogen.Application;

public abstract class ApplicationFinalizerBase : IApplicationFinalizer {
	public const int DefaultPriority = 100;

	public virtual int Priority => DefaultPriority;

	public virtual bool Parallelizable => false;

	public abstract void Finalize();
}
