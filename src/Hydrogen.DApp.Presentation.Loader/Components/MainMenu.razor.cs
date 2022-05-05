﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Hydrogen.DApp.Presentation.Loader.Components
{
    public partial class MainMenu
    {
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                JsRuntime.InvokeVoidAsync("addDropdownHover");
            }

            return base.OnAfterRenderAsync(firstRender);
        }
    }

}