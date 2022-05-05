using Microsoft.AspNetCore.Components;
using Hydrogen.DApp.Presentation.Components.Modal;
using Hydrogen.DApp.Presentation.Services;

namespace Hydrogen.DApp.Presentation.Loader.Layouts
{
    /// <summary>
    /// Main layout
    /// </summary>
    public partial class MainLayout
    {
        /// <summary>
        /// Modal component reference
        /// </summary>
        private ModalHost? _modal;

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        [Inject] private IModalService ModalService { get; set; } = null!;

        /// <inheritdoc />
        protected override void OnAfterRender(bool firstRender)
        {
            if (_modal is not null)
            {
                ModalService.Initialize(_modal);
            }

            base.OnAfterRender(firstRender);
        }
    }
}