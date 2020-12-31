using Microsoft.JSInterop;

namespace VelocityNET.Presentation.Blazor.ViewModels
{
    public class SidebarViewModel
    {
        public SidebarViewModel(IJSRuntime jsRuntime)
        {
            jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/sb-admin-2.min.js");
        }
    }
}