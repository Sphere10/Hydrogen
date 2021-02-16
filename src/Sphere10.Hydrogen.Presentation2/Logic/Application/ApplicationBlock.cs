using System.Collections.Generic;

namespace Sphere10.Hydrogen.Presentation2.Logic {

	public class ApplicationBlock : IApplicationBlock {

		public const string DefaultIconUrl = "/?";

		public string Title { get; init; }
		public string IconUrl { get; init; }
		public string Tooltip { get; init; }
		public IReadOnlyList<IApplicationMenu> Menus { get; init; }


	}

}
