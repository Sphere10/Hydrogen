using System;
using System.Collections.Generic;

namespace Sphere10.Helium.Message
{
    public interface IMessageHeader
    {
        string Id { get; set; }

        string ReplyToAddress { get; set; }

        IDictionary<string, string> Headers { get; set; }

        TimeSpan? TimeToLive { get; set; }
    }
}
