using System;

namespace Sphere10.Helium.Framework {
	
	public record PluginAssemblyHandler {
		public string RelativePath { get; init; }
		public string AssemblyFullName { get; init; }
		public Type Handler { get; init; }
		public Type Message { get; init; }
		public bool IsEnabled { get; set; } = true;
	}
}
