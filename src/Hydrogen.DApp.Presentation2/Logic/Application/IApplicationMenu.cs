using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation2.Logic {

    public interface IApplicationMenu {
        public string Icon { get; set; }
        public string Text { get; set; }
        IEnumerable<IApplicationMenuItem> Items { get; }
    }
}