using System;

namespace Sphere10.Hydrogen.Presentation.Models {

    public class Block {
        public int Number { get; set; }

        public string Address { get; set; } = Guid.NewGuid().ToString();
    }
}