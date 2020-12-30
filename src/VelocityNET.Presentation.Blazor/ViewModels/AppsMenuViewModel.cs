using System;
using System.Collections.Generic;
using VelocityNET.Presentation.Blazor.Plugins;
using VelocityNET.Presentation.Blazor.Shared;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.ViewModels
{
    /// <summary>
    /// Apps menu view model.
    /// </summary>
    public class AppsMenuViewModel : ComponentViewModelBase
    {
        /// <summary>
        /// Gets the available apps.zs
        /// </summary>
        public IEnumerable<IApp> Apps => AppManager.Apps;
        
        /// <summary>
        /// Gets the navigation manager
        /// </summary>
        private IAppManager AppManager { get; }

        /// <summary>
        /// Initialize an instance of the <see cref="AppsMenuViewModel"/> class.
        /// </summary>
        /// <param name="appManager"></param>
        public AppsMenuViewModel(
            IAppManager appManager)
        {
            AppManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
        }

        /// <summary>
        /// Navigates to the selected app.
        /// </summary>
        /// <param name="appName"> app name.</param>
        public void NavigateToApp(string appName)
        {
            AppManager.SelectApp(appName);
        }
    }
}