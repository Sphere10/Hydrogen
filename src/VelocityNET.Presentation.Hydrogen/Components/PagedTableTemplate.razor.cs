using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace VelocityNET.Presentation.Hydrogen.Components
{

    public partial class PagedTableTemplate<TItem>
    {
        [Parameter] public List<TItem> Items { get; set; }

        private List<TItem> Page => Items.Skip(CurrentPage > 1 ? CurrentPage * PageSize : 0).Take(PageSize).ToList();

        [Parameter] public int PageSize { get; set; } = 25;

        public int CurrentPage { get; set; } = 1;

        public bool HasNextPage => Items.Count / PageSize > CurrentPage;

        public bool HasPrevPage => Items.Count / PageSize < CurrentPage;

        [Parameter] public RenderFragment<TItem> ItemTemplate { get; set; }

        [Parameter] public RenderFragment HeaderTemplate { get; set; }

        private void OnNextPage()
        {
            CurrentPage++;
        }

        private void OnPrevPage()
        {
            CurrentPage--;
        }
    }

}