using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework.Collections.StreamMapped
{

    internal class FixedSizeStreamPage<TItem> : StreamPageBase<TItem>
    {
        private readonly int _item0Offset;
        private int _version;

        public FixedSizeStreamPage(StreamMappedList<TItem> parent) : base(parent)
        {
            _version = 0;

            if (!Serializer.IsFixedSize)
            {
                throw new ArgumentException(
                    $"Parent list's serializer is not fixed size. {nameof(FixedSizeStreamPage<TItem>)} only supports fixed sized items.",
                    nameof(parent));
            }

            _item0Offset = Parent.IncludeListHeader ? StreamMappedList<TItem>.ListHeaderSize : 0;
            
            base.State = PageState.Loaded;
            base.StartIndex = 0;
            
            StartPosition = _item0Offset;
        }
        
        protected int MaxItems => Parent.PageSize / ItemSize;
        
        public override int Count => (int)(Stream.Length - _item0Offset) / ItemSize;

        public override int Size => Count * ItemSize;

        public override IEnumerator<TItem> GetEnumerator()
        {
            var currentVersion = _version;

            void CheckVersion()
            {
                if (currentVersion != _version)
                    throw new InvalidOperationException("Page was changed during enumeration");
            }

            return ReadInternal(StartIndex, Count)
                .GetEnumerator()
                .OnMoveNext(CheckVersion);
        }

        protected override IEnumerable<TItem> ReadInternal(int index, int count)
        {
            int startIndex = index * ItemSize + _item0Offset;

            for (int i = 0; i < count; i++)
            {
                Stream.Seek(startIndex + i * ItemSize, SeekOrigin.Begin);
                
                yield return Serializer.Deserialize(ItemSize, Reader);
            }
        }

        protected override int AppendInternal(TItem[] items, out int newItemsSize)
        {
            if (items.Length + Count > MaxItems)
            {
                throw new InvalidOperationException("Unable to append items, Max Items of page will be exceeded.");
            }
            
            Stream.Seek(_item0Offset + Count * ItemSize, SeekOrigin.Begin);

            foreach (TItem item in items)
            {
                Serializer.Serialize(item, Writer);
            }

            newItemsSize = items.Length * ItemSize;
            return items.Length;
        }

        protected override void UpdateInternal(int index, TItem[] items, out int oldItemsSize, out int newItemsSize)
        {
            int itemsSize = items.Length * ItemSize;
            index = index * ItemSize + _item0Offset;
            
            Stream.Seek(index, SeekOrigin.Begin);

            foreach (TItem item in items)
            {
                Serializer.Serialize(item, Writer);
            }

            newItemsSize = itemsSize;
            oldItemsSize = itemsSize;
        }

        protected override void EraseFromEndInternal(int count, out int oldItemsSize)
        {
            int erasedBytes = ItemSize * count;
            Stream.SetLength(Stream.Length - erasedBytes);
            oldItemsSize = erasedBytes;
        }
    }

}