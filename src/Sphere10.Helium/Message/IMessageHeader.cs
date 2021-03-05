using System.Collections.Generic;

namespace Sphere10.Helium.Message
{
    public interface IMessageHeader
    {
        string Id { get; }

        string ReplyToAddress { get; }

        IDictionary<string, string> Headers { get; }
    }
}
