using System;

namespace VelocityNET.Core.DataObjects {
    interface ICommunications {
        event EventHandler PeerAdded;
        event EventHandler PeerRemoved;
        event EventHandler PeerMessage;
    }
}