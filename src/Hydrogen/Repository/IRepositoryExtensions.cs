// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;

namespace Hydrogen;

public static class IRepositoryExtensions {
	public static TEntity Get<TEntity, TIdentity>(this IRepository<TEntity, TIdentity> repo, TIdentity identity) {
		if (!repo.TryGet(identity, out var entity))
			throw new InvalidOperationException($"Entity `{identity}` not found in repository");
		return entity;
	}

	public static async Task<TEntity> GetAsync<TEntity, TIdentity>(this IRepository<TEntity, TIdentity> repo, TIdentity identity) {
		var result = await repo.TryGetAsync(identity);
		if (!result.Item1)
			throw new InvalidOperationException($"Entity `{identity}` not found in repository");
		return result.Item2;
	}

}
