using System;

namespace Sphere10.Hydrogen.Core.DataObjects {
    interface ICommunications {
        event EventHandler PeerAdded;
        event EventHandler PeerRemoved;
        event EventHandler PeerMessage;
    }
}