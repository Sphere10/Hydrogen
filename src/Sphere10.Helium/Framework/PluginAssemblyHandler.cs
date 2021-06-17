using System;

namespace Sphere10.Helium.Framework {
	
	public record PluginAssemblyHandler {
		public string AssemblyFullName { get; init; }
		public Type Handler { get; init; }
		public Type Message { get; init; }
	}
}
