using System.Collections.Generic;

namespace VelocityNET.Presentation.Blazor.Shared.Plugins
{
    public interface IApp : IRoutablePage, INamedItem
    {
        /// <summary>
        /// Gets the app blocks that are part of this 
        /// </summary>
        IEnumerable<IAppBlock> AppBlocks { get; }
    }
}