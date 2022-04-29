using System;
using Hydrogen.DApp.Presentation.Models;

namespace Hydrogen.DApp.Presentation.Events {

    public class DataSourceChangedEventArgs : EventArgs {
        private Uri Server { get; }

        public DataSourceChangedEventArgs(Uri server) {
            Server = server ?? throw new ArgumentNullException(nameof(server));
        }
    }

}