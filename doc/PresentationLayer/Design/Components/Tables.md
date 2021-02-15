# Tables

Reusable generic templated components for quickly creating table views.

## PagedTable

Simple table component with paging and page size selection. Bound to `IEnumerable<T>` item source.

![image-20210215141208877](..\..\resources\Pagedtabled.png)

| Member         | Comments                                                     |
| -------------- | ------------------------------------------------------------ |
| Items          | IEnumerable<T> source of items.                              |
| Class          | An optional parameter on the component to set the CSS classes on the generated table component |
| HeaderTemplate | RenderFragment template for table header                     |
| ItemTemplate   | RenderFragment template applied per row                      |
| OnRowSelect    | Action delegate executed when on row select, receives row model T as parameter. |
| PageSize       | Optional parameter to set the size of pages                  |

```html
// Usage of component, parameters setting

<PagedTable class="table table-borderless table-hover overflow-auto"
                            Items="Blocks"
                            TItem="Block"
                            PageSize="10"
                            OnRowSelect="OnClickRowAsync">
                    <HeaderTemplate>
                        <tr>
                            <th> Address</th>
                            <th class="text-right"> Number</th>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate Context="block">
                        <tr>
                            <td>@block.Address</td>
                            <td class="text-right">@block.Number</td>
                        </tr>
                    </ItemTemplate>
</PagedTable>
```



## VirtualPagedTable

Paging table component whose items are provided "on demand" from an asynchronous data source. 

![image-20210215141317037](..\..\resources\Virtualpagedtable.png)

| Member        | Comments                                                     |
| ------------- | ------------------------------------------------------------ |
| ItemsProvider | An ItemsProviderDelegate instance which is used to obtain the next or previous page |

```html
 <VirtualPagedTable Class="table table-borderless table-hover overflow-auto"
                                   ItemsProvider="request => ViewModel!.NodeService.GetBlocksAsync(request)"
                                   OnRowSelect="OnClickRowAsync"
                                   TItem="Block"
                                   PageSize="10">
                    <HeaderTemplate>
                        <tr>
                            <th> Address</th>
                            <th> Number</th>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate Context="block">
                        <tr>
                            <td>@block.Address</td>
                            <td>@block.Number</td>
                        </tr>
                    </ItemTemplate>
                </VirtualPagedTable>
```



## RapidTable

A table with fixed row size with an asynchronous enumerable data source that is iterated until completed or cancelled. Useful for live update data sources.

![image-20210215142014934](..\..\resources\Rapditable.png)

| Member    | Comment                                                      |
| --------- | ------------------------------------------------------------ |
| Source    | IAsyncEnumerable<TItem> data source from a stream, channel etc. |
| ItemLimit | The number of items to persist at a time - first in first out. |

```html
<RapidTable ItemLimit="10"
                            Source="@ViewModel!.NodeService.GetBlocksAsync()"
                            TItem="Block"
                            OnRowSelect="OnClickRowAsync"
                            Class="table table-striped table-borderless table-hover">
                    <HeaderTemplate>
                        <tr>
                            <th> Address</th>
                            <th class="text-right"> Number</th>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate Context="block">
                        <tr>
                            <td>@block.Address</td>
                            <td class="text-right">@block.Number</td>
                        </tr>
                    </ItemTemplate>
                </RapidTable>
```