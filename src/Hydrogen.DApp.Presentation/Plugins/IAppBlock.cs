using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation.Plugins {
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