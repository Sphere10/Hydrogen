using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation2.Logic {

	public class ApplicationBlock : IApplicationBlock {

		public const string DefaultIconUrl = "/?";

		public int Position { get; } = 0;
		public string Title { get; init; }
		public string IconUrl { get; init; }
		public string Tooltip { get; init; }
		public IReadOnlyList<IApplicationMenu> Menus { get; init; }


	}

}
