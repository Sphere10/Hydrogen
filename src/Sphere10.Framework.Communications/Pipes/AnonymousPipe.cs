using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using Sphere10.Framework;
using Sphere10.Framework.Scheduler;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {

    public abstract class AnonymousPipe : ProtocolChannel, IDisposable {
	    public new static readonly byte[] MessageEnvelopeMarker = { 0, 0, 0, 1 };

        public event EventHandlerEx<string> ReceivedString;
        public event EventHandlerEx<string> SentString;

        private PipeStream _readStream;
        private PipeStream _writeStream;
        private StreamReader _reader;
        private StreamWriter _writer;

        protected AnonymousPipe() {
            base.MessageEnvelopeMarker = MessageEnvelopeMarker;
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
        public static AnonymousPipe ToChildProcess(string processPath, IItemSerializer<object> serializer, string arguments = "", Func<string, string, string, string> argInjectorFunc = null)
            => new AnonymousServerPipe(processPath, arguments, argInjectorFunc) {
                MessageSerializer = serializer,
                MessageSerializationEnabled = true
            };

		public static AnonymousPipe FromChildProcess(AnonymousPipeEndpoint serverEndpoint, IItemSerializer<object> serializer) 
			=> new AnonymousClientPipe(serverEndpoint) {
				MessageSerializer = serializer,
                MessageSerializationEnabled = true
			};

        public override int MinMessageLength => 0;

        public override int MaxMessageLength => 1 << 16;

        public AnonymousPipeEndpoint Endpoint { get; protected set; }

        public async Task<bool> TrySendString(string @string, CancellationToken cancellationToken) {
            Guard.ArgumentNotNull(@string, nameof(@string));
	        if (!State.IsIn(ProtocolChannelState.Opening, ProtocolChannelState.Open) || !IsConnectionAlive())
		        return false;
	        try {
		        await Task.Run(_writeStream.WaitForPipeDrain, cancellationToken); // ensure last message was read before new sent
		        await _writer.WriteLineAsync(@string.AsMemory(), cancellationToken);
		        NotifySentString(@string);
		        return true;
	        } catch {
	        }
	        return false;
        }

        public void Dispose() {
            _writeStream?.Dispose();
            _readStream?.Dispose();
        }

        protected override async Task OpenInternal() {
	        (Endpoint, _readStream, _writeStream) = await OpenPipeInternal();
	        _writer = new StreamWriter(_writeStream) { AutoFlush = true };
	        _reader = new StreamReader(_readStream);
        }

        protected abstract Task<(AnonymousPipeEndpoint endpoint, PipeStream readStream, PipeStream writeStream)> OpenPipeInternal();

        protected override async Task CloseInternal() {
	        await _writer.DisposeAsync();
	        _reader.Dispose();
	        await _readStream.DisposeAsync();
	        await _writeStream.DisposeAsync();
        }

        protected override bool IsConnectionAlive() => _readStream.IsConnected && _writeStream.IsConnected;

        protected override async Task<bool> TryWaitHandshake(CancellationToken cancellationToken) {
	        return LocalRole switch {
		        CommunicationRole.Server => await TrySendString("SYNC", cancellationToken) && await TryWaitForString("SYNC", cancellationToken),
		        CommunicationRole.Client => await TryWaitForString("SYNC", cancellationToken) && await TrySendString("SYNC", cancellationToken),
		        _ => false
	        };
        }

        protected override Task<bool> TrySendBytesInternal(ReadOnlySpan<byte> bytes, CancellationToken cancellationToken) 
            => TrySendString(Convert.ToBase64String(bytes), cancellationToken);

        protected override async Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken) {
	        var str = await ReceiveString(cancellationToken);
	        if (string.IsNullOrEmpty(str))
		        return Array.Empty<byte>();
            return Convert.FromBase64String(str);
        }

        protected virtual void OnReceivedString(string @string) {
        }

        protected virtual void OnSentString(string @string) {
        }

        public async Task<bool> TryWaitForString(string value, CancellationToken cancellationToken) {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => {
	            taskCompletionSource.TrySetResult(false);
            });

            void MonitorSync(string @string) {
                if (@string == value)
                    taskCompletionSource.TrySetResult(true);
                this.ReceivedString -= MonitorSync;
            }
            this.ReceivedString += MonitorSync;
            
            return await taskCompletionSource.Task;
        }

        private async Task<string> ReceiveString(CancellationToken cancellationToken) {
            var @string = await _reader.ReadLineAsync().WithCancellationToken(cancellationToken).IgnoringCancellationException();
            if (!string.IsNullOrEmpty(@string))
				NotifyReceivedString(@string);
            return @string;
        }

        private void NotifyReceivedString(string str) {
            OnReceivedString(str);
            ReceivedString?.Invoke(str);
        }

        private void NotifySentString(string str) {
            OnSentString(str);
            SentString?.Invoke(str);
        }

    }
    

}
