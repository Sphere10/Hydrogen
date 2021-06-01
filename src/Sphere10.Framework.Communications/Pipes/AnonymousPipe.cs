using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using Sphere10.Framework.Communications.Protocol;
using Sphere10.Framework;
using Sphere10.Framework.Scheduler;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {

    public abstract class AnonymousPipe : ProtocolChannel<AnonymousPipeEndpoint, IAnonymousPipeMessage>, IDisposable {

	    public event EventHandlerEx<object, string> ReceivedString;
        public event EventHandlerEx<object, string> SentString;

        private PipeStream _readStream;
        private PipeStream _writeStream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private readonly CancellationTokenSource _cancelConnection;
        private bool _tmpVal;

        protected AnonymousPipe() {
            _cancelConnection = new CancellationTokenSource();
        }

        /// <summary>
        /// Starts a child process and passes in the read/writer pipe handles.
        /// </summary>
        /// <param name="processPath">Path to the child process</param>
        /// <param name="arguments">Arguments to pass into the child process</param>
        /// <param name="argInjectorFunc">A callback which will inject the read and write pipe handle into the <paramref name="arguments"/>. The first argument of <paramref name="argInjectorFunc"/> is the entire arguments string (empty if none), the second argument is the server read pipe handle, the third argument is the server write pipe handle, the return value is the final argument string to pass into the process with read/write pipe handles injected.</param>
        /// <param name="mediator">Handles bad messages.</param>
        /// <returns>A channel used to send messages backwards and forwards</returns>
        /// <remarks>If <paramref name="argInjectorFunc"/> is null the read pipe handle and write pipe handle are passed consequtively.</remarks>
        public static AnonymousPipe ToChildProcess(string processPath, FactorySerializer<IAnonymousPipeMessage> serializer, string arguments = "", Func<string, string, string, string> argInjectorFunc = null)
            => new AnonymousServerPipe(processPath, arguments, argInjectorFunc) {
                MessageSerializer = serializer,
            };

		public static AnonymousPipe FromChildProcess(AnonymousPipeEndpoint serverEndpoint, FactorySerializer<IAnonymousPipeMessage> serializer) 
			=> new AnonymousClientPipe(serverEndpoint) {
				MessageSerializer = serializer,
			};

        public override int MinMessageLength => 0;

        public override int MaxMessageLength => 1 << 16;

        public async Task SendString(string @string) {
            //await _writer.WriteLineAsync(@string);
            _writer.WriteLine(@string);
            _writeStream.WaitForPipeDrain();
            NotifySentString(@string);
        }

        public void Dispose() {
            _writer?.Flush();
            _writeStream?.Dispose();
            _readStream?.Dispose();
        }

        protected override async Task OpenInternal() {
	        (Endpoint, _readStream, _writeStream) = await OpenPipeInternal();
	        _writer = new StreamWriter(_writeStream);
	        _reader = new StreamReader(_readStream);
            _tmpVal = MessageSerializationEnabled;
            MessageSerializationEnabled = false;
        }

        protected override async Task WaitHandshake() {
            try {
                //switch (LocalRole) {
                //    case CommunicationRole.Server:
                //        await SendSYNC();
                //        await ReceiveSYNC();
                //        break;
                //    case CommunicationRole.Client:
                //        await ReceiveSYNC();
                //        await SendSYNC();
                //        break;
                //}
            } finally {
                MessageSerializationEnabled = _tmpVal;
            }
        }

        protected abstract Task<(AnonymousPipeEndpoint endpoint, PipeStream readStream, PipeStream writeStream)> OpenPipeInternal();

        protected override async Task CloseInternal() {
	        //_readStream.Close();
	        //_writeStream.Close();
	        await _readStream.DisposeAsync();
	        await _writeStream.DisposeAsync();
	        await _writer.DisposeAsync();
	        _reader.Dispose();
        }

        protected override Task SendBytesInternal(ReadOnlySpan<byte> bytes) 
            => SendString(Convert.ToBase64String(bytes));

        protected override async Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken) 
            => Convert.FromBase64String(await ReceiveString(cancellationToken));

        protected virtual void OnReceivedString(string @string) {
        }

        protected virtual void OnSentString(string @string) {
        }

        public Task SendSYNC() => SendString("SYNC");

        public async Task ReceiveSYNC() {
            var task = new TaskCompletionSource();
            void MonitorSync(object sender, string @string) {
                if (@string == "SYNC")
                    task.TrySetResult();
                this.ReceivedString -= MonitorSync;
            }
            this.ReceivedString += MonitorSync;
            await task.Task;
        }


        public async Task<string> ReceiveString(CancellationToken cancellationToken) {
            //var @string = await _reader.ReadLineAsync();//.WithCancellationToken(cancellationToken);
            //var @string = await Task.Run( () => _reader.ReadLine());//.WithCancellationToken(cancellationToken);
            //var @string = _reader.ReadLine();//.WithCancellationToken(cancellationToken);
            //NotifyReceivedString(@string);
            //return @string;

            while (true) {
                var str = _reader.ReadLine();
                NotifyReceivedString(str);
            }
        }


        private void NotifyReceivedString(string str) {
            OnReceivedString(str);
            ReceivedString?.Invoke(this, str);
        }

        private void NotifySentString(string str) {
            OnSentString(str);
            SentString?.Invoke(this, str);
        }

    }
    

}
