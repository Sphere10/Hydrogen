using System.IO.Pipes;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {
    public sealed class AnonymousClientPipe : AnonymousPipe {

        public AnonymousClientPipe(AnonymousPipeEndpoint endpoint) {
            Guard.ArgumentNotNull(endpoint, nameof(endpoint));
            this.Endpoint = endpoint;
        }

        public override CommunicationRole LocalRole => CommunicationRole.Client;

        public override CommunicationRole Initiator => CommunicationRole.Server;

        protected override async Task<(AnonymousPipeEndpoint endpoint, PipeStream readStream, PipeStream writeStream)> OpenPipeInternal() {
            var readPipe = new AnonymousPipeClientStream(PipeDirection.In, Endpoint.WriterHandle);
            var writePipe = new AnonymousPipeClientStream(PipeDirection.Out, Endpoint.ReaderHandle);
			return (Endpoint, readPipe, writePipe);
        }

    }
    

}
