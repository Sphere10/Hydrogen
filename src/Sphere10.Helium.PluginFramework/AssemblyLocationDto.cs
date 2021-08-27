using System.Reflection;

namespace Sphere10.Helium.PluginFramework {
	public record AssemblyLocationDto {
		public Assembly Assembly { get; init; }
		public string FullPath { get; init; }
		public string RelativePath { get; init; }
	}
}