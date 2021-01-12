using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace VelocityNET.Presentation.Hydrogen.Components
{
    public partial class PagedTableTemplate<TItem>
    {
        [Parameter]
        public IEnumerable<TItem> Items
        {
            get => ViewModel!.Items;
            set => ViewModel!.Items = value;
        }

        [Parameter]
        public int PageSize
        {
            get => ViewModel!.PageSize;
            set => ViewModel!.PageSize = value;
        }

        [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; } 

        [Parameter] public RenderFragment? HeaderTemplate { get; set; }
    }

}