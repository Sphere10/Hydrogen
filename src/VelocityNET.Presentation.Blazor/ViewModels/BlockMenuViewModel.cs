using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using VelocityNET.Presentation.Blazor.Plugins;
using VelocityNET.Presentation.Blazor.Shared;
using VelocityNET.Presentation.Blazor.Shared.Plugins;
using VelocityNET.Presentation.Blazor.Shared.ViewModels;

namespace VelocityNET.Presentation.Blazor.ViewModels
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
        private IApp SelectedApp => AppManager.SelectedApp;

        /// <summary>
        /// Gets the app blocks for the selected app
        /// </summary>
        public IEnumerable<IAppBlock> AppBlocks => SelectedApp.AppBlocks ?? Enumerable.Empty<IAppBlock>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockMenuViewModel"/> class.
        /// </summary>
        /// <param name="appManager"></param>
        public BlockMenuViewModel(IAppManager appManager)
        {
            AppManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
            AppManager.AppSelected += AppManagerOnAppSelected;
        }

        /// <summary>
        /// Handles app selected event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppManagerOnAppSelected(object? sender, AppSelectedEventArgs e)
        {
            StateHasChangedDelegate?.Invoke();
        }
    }
}