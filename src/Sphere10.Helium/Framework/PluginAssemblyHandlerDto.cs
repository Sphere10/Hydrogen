using System;

namespace Sphere10.Helium.Framework {
	
	public record PluginAssemblyHandlerDto {
		public string RelativePath { get; init; }
		public string FullPath { get; set; }
		public string AssemblyFullName { get; init; }
		public Type HandlerClass { get; init; }
		public Type HandlerInterface { get; init; }
		public Type Message { get; init; }
		public bool IsEnabled { get; set; } = true;
	}
}
