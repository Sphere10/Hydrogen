using Microsoft.AspNetCore.Components;

namespace VelocityNET.Presentation.Hydrogen.Components.Modal
{
    public partial class ModalTemplate
    {
        [Parameter]
        public RenderFragment? HeaderTemplate { get; set; }

        [Parameter]
        public RenderFragment? FooterTemplate { get; set; }
        
        [Parameter]
        public RenderFragment? BodyTemplate { get; set; }
    }
}