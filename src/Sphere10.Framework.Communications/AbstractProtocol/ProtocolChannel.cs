using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {

    public abstract class ProtocolChannel : IAsyncDisposable {
        public const int DefaultTimeoutMS = 5000;
        private const int DefaultMinMessageLength = 0;
        private const int DefaultMaxMessageLength = 65536;

        // Events
        public event EventHandlerEx Opening;
        public event EventHandlerEx Opened;
        public event EventHandlerEx Closing;
        public event EventHandlerEx Closed;
        public event EventHandlerEx Handshake;
        public event EventHandlerEx<ReadOnlyMemory<byte>> ReceivedBytes;
        public event EventHandlerEx<Memory<byte>> SentBytes;
        public event EventHandlerEx<ProtocolMessageEnvelope> ReceivedMessage;
        public event EventHandlerEx<ProtocolMessageEnvelope> SentMessage;

        // Fields
        protected CancellationTokenSource _cancelReceiveLoop;
        private Task _receiveLoop;
        private volatile int _messageID;

        // Constructors
        protected ProtocolChannel() {
            _cancelReceiveLoop = null;
            _receiveLoop = null;
            MessageEnvelopeMarker = Array.Empty<byte>();
            DefaultTimeout = TimeSpan.FromMilliseconds(DefaultTimeoutMS);
            State = ProtocolChannelState.Closed;
            _messageID = 0;
        }

        #region Properties

        public ProtocolChannelState State { get; private set; }

        public TimeSpan DefaultTimeout { get; set; }

        public abstract CommunicationRole LocalRole { get; }

        public abstract CommunicationRole Initiator { get; }

        public bool MessageSerializationEnabled { get; set; }

        public virtual IItemSerializer<object> MessageSerializer { get; init; }

        public virtual int MinMessageLength { get; init; } = DefaultMinMessageLength;

        public virtual int MaxMessageLength { get; init; } = DefaultMaxMessageLength;

        protected byte[] MessageEnvelopeMarker { get; init; }

        #endregion

        #region Methods

        public async Task Open() {
            CheckState(ProtocolChannelState.Closed);
            SetState(ProtocolChannelState.Opening);
            NotifyOpening();
            _cancelReceiveLoop = new CancellationTokenSource();
            _receiveLoop = new Task(async () => await ReceiveLoop(_cancelReceiveLoop.Token));
            await OpenInternal();
            _receiveLoop.Start();
            var cancelHandshake = new CancellationTokenSource(DefaultTimeout);
            if (await TryWaitHandshake(cancelHandshake.Token)) {
                SetState(ProtocolChannelState.Open);
                NotifyHandshake();
                NotifyOpened();
            } else {
                SetState(ProtocolChannelState.Closing);
                NotifyClosing();
                _cancelReceiveLoop.Cancel(false);
                await CloseInternal();
            }
        }

        public async Task Close() {
            const int forceCancelReceiveLoopMS = 150;
            CheckState(ProtocolChannelState.Open, ProtocolChannelState.Closing, ProtocolChannelState.Closed);
            if (State == ProtocolChannelState.Closed)
                return;
            if (State != ProtocolChannelState.Closing) {
                SetState(ProtocolChannelState.Closing);
                NotifyClosing();
            }
            _cancelReceiveLoop.CancelAfter(forceCancelReceiveLoopMS);
            try {
                // Receive loop SHOULD immediately react to channel State being Closed.
                // But if it does not, then after some time (forceCancelReceiveLoopMS), it is forced closed.
                await _receiveLoop;
                if (State != ProtocolChannelState.Closed) {
                    SetState(ProtocolChannelState.Closed);
                    NotifyClosed();
                }
            } catch (TaskCanceledException) {
            }

            await CloseInternal();
            // Note: NotifyClosed() is called by receive loop termination
        }

        public virtual Task<bool> TrySendBytes(byte[] bytes) => TrySendBytes(bytes, DefaultTimeout);

        public virtual Task<bool> TrySendBytes(byte[] bytes, TimeSpan timeout) {
            var canceller = new CancellationTokenSource(timeout);
            return TrySendBytes(bytes, canceller.Token);
        }

        public async Task<bool> TrySendBytes(byte[] bytes, CancellationToken cancellationToken) {
            if (!IsConnectionAlive())
                return false;

            if (!await TrySendBytesInternal(bytes, cancellationToken))
                return false;
            NotifySentBytes(bytes);
            return true;
        }

        public virtual Task<bool> TrySendMessage(ProtocolMessageType messageType, object message)
            => TrySendMessage(messageType, message, DefaultTimeout);

        public virtual Task<bool> TrySendMessage(ProtocolMessageType messageType, object message, TimeSpan timeout) {
            var canceller = new CancellationTokenSource(timeout);
            return TrySendMessage(messageType, message, canceller.Token);
        }

        public virtual async Task<bool> TrySendMessage(ProtocolMessageType messageType, object message, CancellationToken cancellationToken) {

            Guard.Ensure(MessageSerializationEnabled, "Message Serialization is not enabled");
            Guard.Ensure(MessageSerializer != null, "Message Serializer is not set");
            var envelope = new ProtocolMessageEnvelope {
                MessageType = messageType,
                RequestID = Interlocked.Increment(ref _messageID),
                Message = message
            };
            return await TrySendEnvelope(envelope, cancellationToken);
        }

        internal virtual async Task<bool> TrySendEnvelope(ProtocolMessageEnvelope envelope, CancellationToken cancellationToken) {
            using var memStream = new MemoryStream();
            using var writer = new EndianBinaryWriter(EndianBitConverter.Little, memStream);
            writer.Write(MessageEnvelopeMarker);
            writer.Write((byte)envelope.MessageType);
            writer.Write(envelope.RequestID);
            writer.Write((uint)MessageSerializer.CalculateSize(envelope.Message));
            MessageSerializer.Serialize(envelope.Message, writer);
            if (await TrySendBytes(memStream.GetBuffer(), cancellationToken)) {
                NotifySentMessage(envelope);
                return true;
            }
            return false;
        }

        public async ValueTask DisposeAsync() {
            if (State == ProtocolChannelState.Open)
                await Close();
        }

        // Template-Pattern abstract methods
        protected abstract Task OpenInternal();

        protected abstract Task CloseInternal();

        protected virtual async Task<bool> TryWaitHandshake(CancellationToken cancellationToken) => true;

        protected virtual async Task ReceiveLoop(CancellationToken cancellationToken) {
            CheckState(ProtocolChannelState.Opening, ProtocolChannelState.Open);
            while (State.IsIn(ProtocolChannelState.Opening, ProtocolChannelState.Open) && IsConnectionAlive() && !cancellationToken.IsCancellationRequested) {
                var bytes = await ReceiveBytesInternal(cancellationToken).IgnoringCancellationException();
                if (bytes?.Length > 0)
                    NotifyReceivedBytes(bytes);
            }
            // Connection is Closed only when receive loop finishes
            Guard.Ensure(State != ProtocolChannelState.Closed);
            SetState(ProtocolChannelState.Closed);
            NotifyClosed();
        }

        protected abstract Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken);

        protected abstract Task<bool> TrySendBytesInternal(ReadOnlySpan<byte> bytes, CancellationToken cancellationToken);

        protected abstract bool IsConnectionAlive();

        #endregion

        #region Event handler methods

        protected virtual void OnOpening() {
        }

        protected virtual void OnOpened() {
        }

        protected virtual void OnClosing() {
        }

        protected virtual void OnClosed() {
        }

        protected virtual void OnHandshake() {
        }

        protected virtual void OnReceivedBytes(ReadOnlySpan<byte> bytes) {
            if (!MessageSerializationEnabled)
                return;
            Guard.Ensure(MessageSerializer != null, "Message Serializer is not set");

            using var readStream = new MemoryStream(bytes.ToArray()); // TODO: uses slow ToArray
            using var reader = new EndianBinaryReader(EndianBitConverter.Little, readStream);

            if (readStream.Length < MessageEnvelopeMarker.Length)
                return; // Not an object message

            // Read magic header for message object 
            var magicID = reader.ReadBytes(MessageEnvelopeMarker.Length);
            if (!magicID.AsSpan().SequenceEqual(MessageEnvelopeMarker))
                return; // Message Magic ID not found, so not a message

            // Read envelope
            var messageType = (ProtocolMessageType)reader.ReadOrThrow<byte>(() => new ProtocolException($"Malformed message (missing MessageType)"));
            var requestID = reader.ReadOrThrow<int>(() => new ProtocolException($"Malformed message (missing RequestiD)"));
            var messageLength = reader.ReadOrThrow<uint>(() => new ProtocolException($"Malformed message (missing Object Length))"));

            // Deserialize message
            object message;
            try {
                message = MessageSerializer.Deserialize((int)messageLength, reader);
            } catch (Exception error) {
                throw new ProtocolException($"Malformed message (unable to deserialize message)", error);
            }

            var messageEnvelope = new ProtocolMessageEnvelope {
                MessageType = messageType,
                RequestID = requestID,
                Message = message
            };

            NotifyReceivedMessage(messageEnvelope);
        }

        protected virtual void OnSentBytes(ReadOnlySpan<byte> bytes) {
        }

        protected virtual void OnReceivedMessage(ProtocolMessageEnvelope messageEnvelope) {
        }

        protected virtual void OnSentMessage(ProtocolMessageEnvelope messageEnvelope) {
        }

        #endregion

        #region Notify methods

        private void NotifyOpening() {
            OnOpening();
            Tools.Threads.RaiseAsync(Opening);
        }

        private void NotifyOpened() {
            OnOpened();
            Tools.Threads.RaiseAsync(Opened);
        }

        private void NotifyClosing() {
            OnClosing();
            Tools.Threads.RaiseAsync(Closing);
        }

        private void NotifyClosed() {
            OnClosed();
            Tools.Threads.RaiseAsync(Closed);
        }

        private void NotifyHandshake() {
            OnHandshake();
            Tools.Threads.RaiseAsync(Handshake);
        }

        private void NotifyReceivedBytes(byte[] bytes) {
            OnReceivedBytes(bytes);
            Tools.Threads.RaiseAsync(ReceivedBytes, bytes);
        }

        private void NotifySentBytes(byte[] bytes) {
            OnSentBytes(bytes);
            Tools.Threads.RaiseAsync(SentBytes, bytes);
        }

        private void NotifyReceivedMessage(ProtocolMessageEnvelope messageEnvelope) {
            OnReceivedMessage(messageEnvelope);
            Tools.Threads.RaiseAsync(ReceivedMessage, messageEnvelope);
        }

        private void NotifySentMessage(ProtocolMessageEnvelope messageEnvelope) {
            OnSentMessage(messageEnvelope);
            Tools.Threads.RaiseAsync(SentMessage, messageEnvelope);
        }

        #endregion

        #region Aux 

        private void CheckState(params ProtocolChannelState[] expectedStates) {
            if (!State.IsIn(expectedStates))
                throw new InvalidOperationException($"Channel state was not in {expectedStates.ToDelimittedString(", ")}");
        }


        private void SetState(ProtocolChannelState state) {
            State = state;
        }

        #endregion

    }


}
