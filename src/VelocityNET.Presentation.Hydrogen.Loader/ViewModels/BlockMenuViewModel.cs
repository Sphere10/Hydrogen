using System;
using System.Collections.Generic;
using System.Linq;
using VelocityNET.Presentation.Hydrogen.Loader.Plugins;
using VelocityNET.Presentation.Hydrogen.Plugins;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Loader.ViewModels
{

    /// <summary>
    /// Block menu view model
    /// </summary>
    public class BlockMenuViewModel : ComponentViewModelBase
    {
        /// <summary>
        /// Gets the app manager
        /// </summary>
        private IAppManager AppManager { get; }

        /// <summary>
        /// Gets or sets the selected app.
        /// </summary>
        public IApp? SelectedApp { get; set; }
        
        /// <summary>
        /// Gets or sets the selected app block.
        /// </summary>
        public IAppBlock? SelectedAppBlock { get; set; }
        
        /// <summary>
        /// Gets the app blocks for the selected app
        /// </summary>
        public IEnumerable<IAppBlock> AppBlocks => SelectedApp?.AppBlocks ?? Enumerable.Empty<IAppBlock>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockMenuViewModel"/> class.
        /// </summary>
        /// <param name="appManager"></param>
        public BlockMenuViewModel(IAppManager appManager)
        {
            AppManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
            AppManager.AppSelected += AppManagerOnAppSelected;
            AppManager.AppBlockPageSelected += AppManagerOnAppBlockPageSelected;

            SelectedApp = appManager.SelectedApp;
            SelectedAppBlock = appManager.SelectedApp?.AppBlocks.FirstOrDefault(x =>
                x.AppBlockPages.Any(y => y.Route == appManager.SelectedPage?.Route));
            
            StateHasChangedDelegate?.Invoke();
        }

        /// <summary>
        /// Handles page selected event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppManagerOnAppBlockPageSelected(object? sender, AppBlockPageSelectedEventArgs e)
        {
            SelectedAppBlock = AppManager.SelectedApp!.AppBlocks.First(x =>
                x.AppBlockPages.Any(y => y.Route == e.AppBlockPage.Route));
            StateHasChangedDelegate?.Invoke();
        }

        /// <summary>
        /// Handles app selected event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppManagerOnAppSelected(object? sender, AppSelectedEventArgs e)
        {
            SelectedApp = e.SelectedApp;
            StateHasChangedDelegate?.Invoke();
        }
    }
}