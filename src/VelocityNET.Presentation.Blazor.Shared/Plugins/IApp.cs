using System.Collections.Generic;
using VelocityNET.Presentation.Blazor.Shared.ViewModels;

namespace VelocityNET.Presentation.Blazor.Shared.Plugins
{
    public interface IApp : IRoutablePage, INamedItem, IIconItem
    {
        /// <summary>
        /// Gets the app blocks that are part of this 
        /// </summary>
        IEnumerable<IAppBlock> AppBlocks { get; }
        
        /// <summary>
        /// Gets this apps menu items.
        /// </summary>
        IEnumerable<MenuItem> MenuItems { get; }
    }
}