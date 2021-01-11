using System;

namespace VelocityNET.Presentation.Hydrogen.Models
{

    public class Block
    {
        public int Number { get; set; }

        public string Address { get; set; } = Guid.NewGuid().ToString();
    }
}