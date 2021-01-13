using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.Models;

namespace VelocityNET.Presentation.Hydrogen.Components
{

    public partial class VirtualPagedTableTemplate<TItem>
    {
        public delegate Task<ItemsResponse<TItem>> ItemsProviderDelegate(ItemRequest request);
        
        [Parameter]
        public ItemsProviderDelegate ItemsProvider
        {
            get => ViewModel!.ItemsProvider;
            set => ViewModel!.ItemsProvider = value;
        }
        
        [Parameter]
        public int PageSize
        {
            get => ViewModel!.PageSize;
            set => ViewModel!.PageSize = value;
        }

        [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; } 

        [Parameter] public RenderFragment? HeaderTemplate { get; set; }

        [Parameter] public EventCallback<TItem> OnRowSelect { get; set; } = EventCallback<TItem>.Empty;
    }
}