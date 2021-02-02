﻿using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace VelocityNET.Presentation.Hydrogen.Components
{

    /// <summary>
    /// Rapid table control / component. asyncronously enumerates an async enumerable e.g. a stream
    /// or channel until cancelled or disposed. Generates a table with thead, and tbody from templates
    /// </summary>
    public class RapidTable<TItem> : ComponentWithViewModel<RapidTableViewModel<TItem>>
    {
        /// <summary>
        /// Gets or sets the async item source that will be enumerated.
        /// </summary>
        [Parameter]
        public IAsyncEnumerable<TItem> Source
        {
            get => ViewModel!.Source;
            set => ViewModel!.Source = value;
        }

        /// <summary>
        /// Gets or sets the item template
        /// </summary>
        [Parameter]
        public RenderFragment<TItem> ItemTemplate { get; set; } = null!;

        /// <summary>
        /// Gets or sets the header template
        /// </summary>
        [Parameter]
        public RenderFragment HeaderTemplate { get; set; } = null!;

        /// <summary>
        /// Gets or sets the number of items to be displayed in the table.
        /// </summary>
        [Parameter]
        public int ItemLimit
        {
            get => ViewModel.ItemLimit;
            set => ViewModel.ItemLimit = value;
        }

        /// <summary>
        /// Gets or sets a collection of additional attributes that will be applied to the created element.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token used to cancel the async enumeration of the source property.
        /// </summary>
        [Parameter]
        public CancellationToken CancellationToken
        {
            get => ViewModel!.CancellationToken;
            set => ViewModel!.CancellationToken = value;
        }
        
        /// <summary>
        /// Gets a CSS class string that combines the <c>class</c> attribute
        /// Derived components should typically use this value for the primary HTML element's
        /// 'class' attribute.
        /// </summary>
        protected string CssClass
        {
            get
            {
                if (AdditionalAttributes != null &&
                    AdditionalAttributes.TryGetValue("class", out var @class) &&
                    !string.IsNullOrEmpty(Convert.ToString(@class)))
                {
                    return (string) @class;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "table");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", CssClass);

            builder.OpenElement(3, "thead");
            HeaderTemplate(builder);
            builder.CloseElement();

            builder.OpenElement(4, "tbody");

            foreach (TItem item in ViewModel!.Items)
            {
                ItemTemplate(item)(builder);
            }
            
            builder.CloseElement();
            builder.CloseElement();
        }

        /// <summary>
        /// Method invoked when the component has received parameters from its parent in
        /// the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected override void OnParametersSet()
        {
            if (ItemTemplate is null)
            {
                throw new InvalidOperationException("Item template parameter is required.");
            }
        
            if (HeaderTemplate is null)
            {
                throw new InvalidOperationException("Header template parameter is required.");
            }

            if (Source is null)
            {
                throw new InvalidOperationException("Source parameter is required.");
            }

            base.OnParametersSet();
        }
    }

}