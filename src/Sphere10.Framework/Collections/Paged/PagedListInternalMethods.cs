using System;
using System.Collections.Generic;

namespace Sphere10.Framework
{

    public class PagedListInternalMethods<TItem> : IPagedListInternalMethods<TItem>
    {
        public PagedListInternalMethods(Action checkRequiresLoad, Action<int, int> checkRange,
            Func<IPage<TItem>, IDisposable> enterOpenPageScope,
            Func<int, int, IEnumerable<Tuple<IPage<TItem>, int, int>>> getPageSegments,
            Action notifyAccessing,
            Action notifyAccessed,
            Action<IPage<TItem>> notifyPageAccessing, Action<IPage<TItem>> notifyPageAccessed,
            Action<IPage<TItem>> notifyPageReading, Action<IPage<TItem>> notifyPageRead)
        {
            CheckRequiresLoad = checkRequiresLoad;
            CheckRange = checkRange;
            EnterOpenPageScope = enterOpenPageScope;
            NotifyAccessing = notifyAccessing;
            NotifyPageAccessing = notifyPageAccessing;
            NotifyPageReading = notifyPageReading;
            NotifyPageRead = notifyPageRead;
            NotifyPageAccessed = notifyPageAccessed;
            NotifyAccessed = notifyAccessed;
            GetPageSegments = getPageSegments;
        }

        public Action CheckRequiresLoad { get; }
        
        public Action<int, int> CheckRange { get; }
        
        public Func<IPage<TItem>, IDisposable> EnterOpenPageScope { get; }
        
        public Func<int, int, IEnumerable<Tuple<IPage<TItem>, int, int>>> GetPageSegments { get; }

        public Action NotifyAccessing { get; }
        
        public Action NotifyAccessed { get; }

        public Action<IPage<TItem>> NotifyPageAccessing { get; }
        
        public Action<IPage<TItem>> NotifyPageReading { get; }
        
        public Action<IPage<TItem>> NotifyPageRead { get; }
        
        public Action<IPage<TItem>> NotifyPageAccessed { get; }
    }

}