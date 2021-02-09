using System;
using Sphere10.Hydrogen.Node.Screens;

namespace Sphere10.Hydrogen.Node.UI {

	public class LifetimeAttribute : Attribute {
		public LifetimeAttribute(ScreenLifetime lifetime) {
			Lifetime = lifetime;
		}

		public ScreenLifetime Lifetime { get; private set; }
	}

}
