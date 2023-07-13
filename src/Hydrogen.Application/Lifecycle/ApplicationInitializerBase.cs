// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Application;

public abstract class ApplicationInitializerBase : IApplicationInitializer {
	public const int DefaultPriority = 100;

	public virtual int Priority => DefaultPriority;

	public virtual bool Parallelizable => false;

	public abstract void Initialize();

}
