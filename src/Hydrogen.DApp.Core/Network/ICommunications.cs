using System;

namespace Hydrogen.DApp.Core.DataObjects {
    interface ICommunications {
        event EventHandler PeerAdded;
        event EventHandler PeerRemoved;
        event EventHandler PeerMessage;
    }
}