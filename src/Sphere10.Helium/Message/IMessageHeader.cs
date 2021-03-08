using System;
using System.Collections.Generic;

namespace Sphere10.Helium.Message
{
    public interface IMessageHeader
    {
        string MessageId { get; set; }

        string ReplyToAddress { get; set; }

        TimeSpan? TimeToLive { get; set; }

        DateTime? DateToDie { get; set; }

        IDictionary<string, string> Headers { get; set; }

        MessageHeaderDto GetDefaultMessageHeader();


    }
}
