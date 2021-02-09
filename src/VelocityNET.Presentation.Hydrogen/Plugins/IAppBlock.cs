using System.Collections.Generic;

namespace VelocityNET.Presentation.Hydrogen.Plugins {
    /// <summary>
    /// An app block - provides a set of application functions.
    /// </summary>
    public interface IAppBlock : INamedItem, IIconItem {
        /// <summary>
        /// Gets the pages provided by this app block.
        /// </summary>
        IEnumerable<IAppBlockPage> AppBlockPages { get; }
    }
}