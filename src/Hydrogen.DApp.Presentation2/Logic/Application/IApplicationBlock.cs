using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation2.Logic {
    public interface IApplicationBlock {
		string Title { get; }
		string IconUrl { get; }
		string Tooltip { get;  }
        IReadOnlyList<IApplicationMenu> Menus { get; }
    }

}