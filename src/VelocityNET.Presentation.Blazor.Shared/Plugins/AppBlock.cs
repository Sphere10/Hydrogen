using System;
using System.Collections.Generic;

namespace VelocityNET.Presentation.Blazor.Shared.Plugins
{
    /// <summary>
    /// Application block
    /// </summary>
    public class AppBlock : IAppBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppBlock"/> class.
        /// </summary>
        /// <param name="name"> name</param>
        /// <param name="appBlockPages"> pages</param>
        public AppBlock(string name, IEnumerable<IAppBlockPage> appBlockPages)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AppBlockPages = appBlockPages ?? throw new ArgumentNullException(nameof(appBlockPages));
        }

        /// <summary>
        /// Gets the name of the item, useful for displaying in menus or headings.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        public IEnumerable<IAppBlockPage> AppBlockPages { get; }
    }

}