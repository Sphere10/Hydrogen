using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.Components.Modal;
using VelocityNET.Presentation.Hydrogen.Services;

namespace VelocityNET.Presentation.Hydrogen.Loader.Layouts
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