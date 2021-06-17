using System;

namespace Sphere10.Helium.Framework {
	
	public record PluginAssemblyHandler {
		public Type Handler { get; init; }
		public Type Message { get; init; }
	}
}
