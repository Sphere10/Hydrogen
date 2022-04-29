using System.Collections.Generic;

namespace Sphere10.Hydrogen.Presentation2.Logic {

    public interface IApplicationMenu {
        public string Icon { get; set; }
        public string Text { get; set; }
        IEnumerable<IApplicationMenuItem> Items { get; }
    }
}