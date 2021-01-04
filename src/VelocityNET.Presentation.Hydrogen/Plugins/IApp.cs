using System.Collections.Generic;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Plugins
{
    public interface IApp : IRoutablePage, INamedItem, IIconItem
    {
        /// <summary>
        /// Gets the app blocks that are part of this 
        /// </summary>
        IEnumerable<IAppBlock> AppBlocks { get; }
    }
}