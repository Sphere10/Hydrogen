using System;
using VelocityNET.Presentation.Node.Screens;

namespace VelocityNET.Presentation.Node.UI {

	public class LifetimeAttribute : Attribute {
		public LifetimeAttribute(ScreenLifetime lifetime) {
			Lifetime = lifetime;
		}

		public ScreenLifetime Lifetime { get; private set; }
	}

}
