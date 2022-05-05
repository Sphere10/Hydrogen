using System;
using Hydrogen.DApp.Node.Screens;

namespace Hydrogen.DApp.Node.UI {

	public class LifetimeAttribute : Attribute {
		public LifetimeAttribute(ScreenLifetime lifetime) {
			Lifetime = lifetime;
		}

		public ScreenLifetime Lifetime { get; private set; }
	}

}
