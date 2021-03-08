using System;
using System.Collections.Generic;
using Sphere10.Framework;
using Sphere10.Framework.Collections;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue
{
    public class QueueManager : IQueueManager
    {
        
        public void FirstIn(string destination, IMessage message, string fileName)
        {
            var txnFile = new TransactionalFileMappedBuffer(fileName, 10, 10);
            
            var stream = new ExtendedMemoryStream(txnFile);

            var list = new FixedClusterMappedList<IMessage>(32, 100, 100, new MessageSerializer(), stream);

            list.Add(message);

            throw new NotImplementedException();
        }

        public void LastOut(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void TakeThisMessageOffQueue(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void PersistQueue()
        {
            throw new NotImplementedException();
        }
    }

    internal class MessageSerializer : IObjectSerializer<IMessage>
    {
        public bool IsFixedSize { get; }
        public int FixedSize { get; }
        public int CalculateTotalSize(IEnumerable<IMessage> items, bool calculateIndividualItems, out int[] itemSizes)
        {
            throw new NotImplementedException();
        }

        public int CalculateSize(IMessage item)
        {
            throw new NotImplementedException();
        }

        public int Serialize(IMessage @object, EndianBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public IMessage Deserialize(int size, EndianBinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }

    ////internal class IntSerializer : IObjectSerializer<int>
    ////{
    ////    public bool IsFixedSize { get; } = true;

    ////    public int FixedSize { get; } = BitConverter.GetBytes(0).Length;

    ////    public int CalculateTotalSize(IEnumerable<int> items, bool calculateIndividualItems, out int[] itemSizes)
    ////    {
    ////        throw new NotSupportedException();
    ////    }

    ////    public int CalculateSize(int item) => FixedSize;

    ////    public int Serialize(int @object, EndianBinaryWriter writer)
    ////    {
    ////        writer.Write(BitConverter.GetBytes(@object));
    ////        return sizeof(int);
    ////    }

    ////    public int Deserialize(int size, EndianBinaryReader reader)
    ////    {
    ////        return reader.ReadInt32();
    ////    }
    ////}
}
