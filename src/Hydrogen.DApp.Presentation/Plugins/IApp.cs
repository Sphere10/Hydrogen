﻿using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation.Plugins {
    public interface IApp : IRoutablePage, INamedItem, IIconItem {
        /// <summary>
        /// Gets the app blocks that are part of this 
        /// </summary>
        IEnumerable<IAppBlock> AppBlocks { get; }
    }
}