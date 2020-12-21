using System;

namespace VelocityNET.Presentation.Node {

	public class LifetimeAttribute : Attribute {
		public LifetimeAttribute(ScreenLifetime lifetime) {
			Lifetime = lifetime;
		}

		public ScreenLifetime Lifetime { get; private set; }
	}

}
