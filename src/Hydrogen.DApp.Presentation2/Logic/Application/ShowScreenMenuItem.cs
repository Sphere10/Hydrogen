using System;
using Sphere10.Framework;

namespace Sphere10.Hydrogen.Presentation2.Logic {

	public class ShowScreenMenuItem : ApplicationMenuItem {
		private Type _screenType;

		public Type ScreenType {
			get => _screenType;
			set {
				Guard.ArgumentNotNull(value, nameof(value));
				Guard.Argument(IsValidScreenType(value), nameof(value), "Not an IApplicationScreen");
				_screenType = value;
			}
		}

		public static ShowScreenMenuItem For<TScreen>(string icon, string title) where TScreen : IApplicationScreen 
			=> new ShowScreenMenuItem { Icon = icon, Title = title, ScreenType = typeof(TScreen) };

		private static bool IsValidScreenType(Type screenType) => screenType.IsAssignableTo(typeof(IApplicationScreen));

	}

}
