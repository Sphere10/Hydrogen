// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.Services;

namespace Hydrogen.DApp.Presentation.Loader.Services;

public class BasicGenericEventAggregator : IGenericEventAggregator {
	private Dictionary<Type, List<Delegate>> Subscriptions { get; } = new();


	public void Subscribe<T>(Action<T> handler) {
		if (Subscriptions.ContainsKey(typeof(T))) {
			Subscriptions[typeof(T)].Add(handler);
		} else {
			Subscriptions.Add(typeof(T), new List<Delegate> { handler });
		}
	}

	public void Unsubscribe<T>(Action<T> eventHandler) {
		if (Subscriptions.ContainsKey(typeof(T))) {
			Subscriptions[typeof(T)].Remove(eventHandler);
		}
	}

	public async Task PublishAsync<T>(T data) {
		if (Subscriptions.ContainsKey(typeof(T))) {
			await Task.Run(() => {
				foreach (Delegate o in Subscriptions[typeof(T)]) {
					if (o is Action<T> handler) {
						try {
							handler.Invoke(data);

						} catch (Exception e) {
							Console.WriteLine(e);
						}
					}
				}
			});
		}
	}
}
