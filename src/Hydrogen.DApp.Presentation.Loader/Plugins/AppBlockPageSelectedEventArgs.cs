﻿using System;
using Hydrogen.DApp.Presentation.Plugins;

namespace Hydrogen.DApp.Presentation.Loader.Plugins
{
    /// <summary>
    /// Page selected event args
    /// </summary>
    public class AppBlockPageSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the selected page
        /// </summary>
        public IAppBlockPage AppBlockPage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppBlockPageSelectedEventArgs"/> class.
        /// </summary>
        /// <param name="appBlockPage"> selected page</param>
        public AppBlockPageSelectedEventArgs(IAppBlockPage appBlockPage)
        {
            AppBlockPage = appBlockPage ?? throw new ArgumentNullException(nameof(appBlockPage));
        }
    }
}