using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace VelocityNET.Presentation.Blazor.Shared.Components
{
    /// <summary>
    /// Extends <see cref="ComponentBase"/> adding common functionality and view model support.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public partial class ComponentWithViewModel<TViewModel> : ComponentBase where TViewModel : ComponentViewModelBase
    {
        [Inject]
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        protected TViewModel ViewModel { get; set; }

        protected override Task OnInitializedAsync()
        {
            ViewModel.InitAsync();
            return base.OnInitializedAsync();
        }
    }

}