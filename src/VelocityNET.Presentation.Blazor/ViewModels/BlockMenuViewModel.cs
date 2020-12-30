using System;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Blazor.Shared;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.ViewModels
{

    public class BlockMenuViewModel : ComponentViewModelBase
    {
        private IMessenger Messenger { get; }

        public IApp SelectedApp { get; private set; }

        public BlockMenuViewModel(IMessenger messenger)
        {
            Messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            Messenger.Register<GenericMessage<IApp>>(this, OnAppSelected);
        }

        /// <summary>
        /// Handles the IApp selected message. Updates the block menu with
        /// the selected apps pages.
        /// </summary>
        /// <param name="obj"></param>
        private void OnAppSelected(GenericMessage<IApp> obj)
        {
            SelectedApp = obj.Content;
            StateHasChangedDelegate?.Invoke();
        }
    }
}