using System;

namespace Hydrogen.DApp.Presentation.Models {

    public class Block {
        public int Number { get; set; }

        public string Address { get; set; } = Guid.NewGuid().ToString();
    }
}