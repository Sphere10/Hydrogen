using System;
using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Used to manage versioned serialization of objects by simply registering <see cref="SerializerFactory"/> instances
/// to version numbers. It's up to the user to manage registrations.
/// </summary>
public class VersionedSerializers {
	private readonly IDictionary<int, SerializerFactory> _versions;

	public VersionedSerializers() {
		_versions = new Dictionary<int, SerializerFactory>();
	}

	public SerializerFactory GetFactory(int version) {
		if (!_versions.TryGetValue(version, out var factory))
			throw new InvalidOperationException($"Version {version} not found");
		return factory;
	}

	public void RegisterFactory(int version, SerializerFactory serializer) {
		if (_versions.ContainsKey(version))
			throw new InvalidOperationException($"Version {version} already registered");
		_versions[version] = serializer;
	}
}
