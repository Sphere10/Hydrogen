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

public interface IRepository<TEntity, TIdentity> : IDisposable, IAsyncDisposable {
	event EventHandlerEx<TEntity> Changing;
	event EventHandlerEx<TEntity> Changed;
	event EventHandlerEx<TEntity> Saving;
	event EventHandlerEx<TEntity> Saved;
	event EventHandlerEx Clearing;
	event EventHandlerEx Cleared;
	event EventHandlerEx<TEntity> Adding;
	event EventHandlerEx<TEntity> Added;
	event EventHandlerEx<TEntity> Updating;
	event EventHandlerEx<TEntity> Updated;

	bool Contains(TIdentity identity);

	Task<bool> ContainsAsync(TIdentity identity);

	bool TryGet(TIdentity identity, out TEntity entity);

	Task<(bool, TEntity)> TryGetAsync(TIdentity identity);

	void Create(TEntity entity);

	Task CreateAsync(TEntity entity);

	void Update(TEntity entity);

	Task UpdateAsync(TEntity entity);

	void Delete(TIdentity identity);

	Task DeleteAsync(TIdentity identity);

	void Clear();

	Task ClearAsync();

}