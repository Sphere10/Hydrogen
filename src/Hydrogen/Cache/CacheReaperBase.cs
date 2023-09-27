// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public abstract class CacheReaperBase : ICacheReaper {

	public abstract void Register(ICache cache);

	public abstract void Deregister(ICache cache);

	public abstract long AvailableSpace();

	public abstract long MakeSpace(ICache requestingCache, long requestedBytes);
}
