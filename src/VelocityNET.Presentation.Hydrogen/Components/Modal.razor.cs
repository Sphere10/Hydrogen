using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Components
{

    public partial class Modal
    {
        [Parameter] public RenderFragment Content { get; set; } = null!;
        
        [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

        public async Task<ModalResult> ShowAsync<T>(T component) where T : ModalComponent
        {
            Content = RenderComponent(component);
            StateHasChanged();

            await JsRuntime.InvokeAsync<object>("showModal");
            
            return await component.ShowAsync();
        }

        //https://stackoverflow.com/a/48551735/66988
        private static FieldInfo? GetPrivateField(Type? t, string name)
        {
            const BindingFlags bf = BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly;

            FieldInfo? fi;
            while ((fi = t?.GetField(name, bf)) == null && (t = t?.BaseType) != null)
            {
            }

            return fi;
        }

        private RenderFragment RenderComponent(ComponentBase instance)
        {
            var fragmentField = GetPrivateField(instance.GetType(), "_renderFragment");

            var value = (RenderFragment) fragmentField.GetValue(instance);

            return value;
        }
    }
}