using System.Collections.Generic;

namespace Sphere10.Helium.Bus
{
    public interface IMessageContext
    {
        string Id { get; }

        string ReplyToAddress { get; }

        IDictionary<string, string> Headers { get; }
    }
}
