using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework.Collections.StreamMapped
{

    public class FixedSizeStreamPage<TItem> : PageBase<TItem>
    {
        private readonly StreamMappedList<TItem> _parent;
        private readonly int _item0Offset;
        private int _version;

        public FixedSizeStreamPage(StreamMappedList<TItem> parent)
        {
            _parent = parent;
            _version = 0;

            if (!_parent.Serializer.IsFixedSize)
            {
                throw new ArgumentException(
                    $"Parent list's serializer is not fixed size. {nameof(FixedSizeStreamPage<TItem>)} only supports fixed sized items.",
                    nameof(parent));
            }

            _item0Offset = _parent.IncludeListHeader ? StreamMappedList<TItem>.ListHeaderSize : 0;
            base.State = PageState.Loaded;
            base.StartIndex = 0;
            base.EndIndex = MaxItems - 1;
        }

        protected Stream Stream => _parent.Stream;

        protected EndianBinaryReader Reader => _parent.Reader;

        protected int ItemSize => _parent.Serializer.FixedSize;

        protected IObjectSerializer<TItem> Serializer => _parent.Serializer;

        protected EndianBinaryWriter Writer => _parent.Writer;

        protected int MaxItems => _parent.PageSize / ItemSize;
        
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

            return ReadInternal(StartIndex, Count).GetEnumerator().OnMoveNext(CheckVersion);
        }

        protected override IEnumerable<TItem> ReadInternal(int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                index = (index + i) * ItemSize + _item0Offset;
                Stream.Seek(index, SeekOrigin.Begin);
                yield return Serializer.Deserialize(ItemSize, Reader);
            }
        }

        protected override int AppendInternal(TItem[] items, out int newItemsSize)
        {
            if (items.Length + Count > MaxItems)
            {
                throw new InvalidOperationException("Unable to append items, Max Items of page will be exceeded.");
            }
            
            Stream.Seek(0, SeekOrigin.End);

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