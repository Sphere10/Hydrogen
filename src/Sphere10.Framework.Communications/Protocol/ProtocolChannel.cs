using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {

    public enum ProtocolChannelState {
        Open, 
        Closed
    }

    public abstract class ProtocolChannel : IAsyncDisposable {

        // Events
        public event EventHandlerEx<object> Opening;
        public event EventHandlerEx<object> Opened;
        public event EventHandlerEx<object> Closing;
        public event EventHandlerEx<object> Closed;
        public event EventHandlerEx<object, int> ReceivedBytes;
        public event EventHandlerEx<object, int> SentBytes;

        // Fields
        private CancellationTokenSource _cancelReceiveLoop;
        private Task _receiveLoop;


        // Constructors
        public ProtocolChannel() {
            _receiveLoop = new Task(async () => {
                while (State == ProtocolChannelState.Open) {
                    var bytes = await ReceiveBytesInternal(_cancelReceiveLoop.Token);
                    NotifyReceivedBytes(bytes);
                }
            });
            _cancelReceiveLoop = new CancellationTokenSource();
            State = ProtocolChannelState.Closed;
        }

        // Properties

        public ProtocolChannelState State { get; private set; }

        public virtual object Endpoint { get; protected set; }

        public abstract CommunicationRole LocalRole { get; }

        public abstract CommunicationRole Initiator { get; }

        public abstract int MinMessageLength { get; }

        public abstract int MaxMessageLength { get; }

        // Methods
        public async Task Open() {
            CheckState(ProtocolChannelState.Closed);
            NotifyOpening();
            await OpenInternal();
            State = ProtocolChannelState.Open;
            //_receiveLoop.Start();
            NotifyOpened();
            await WaitHandshake();
        }

        public async Task Close() {
            CheckState(ProtocolChannelState.Open);
            const int forceCancelReceiveLoopMS = 150;
            NotifyClosing();
            State = ProtocolChannelState.Closed;
            _cancelReceiveLoop.CancelAfter(forceCancelReceiveLoopMS);
            try {
                // Receive loop SHOULD immediately react to channel State being Closed.
                // But if it does not, then after some time (forceCancelReceiveLoopMS), it is forced closed.
                await _receiveLoop;
            } catch (TaskCanceledException) {
            }
            await CloseInternal();
            NotifyClosed();
        }

        public async Task SendBytes(byte[] bytes) {
            await SendBytesInternal(bytes);
            NotifySentBytes(bytes);
        }

        public async ValueTask DisposeAsync() {
            if (State == ProtocolChannelState.Open)
                await Close();
        }

        // Template-Pattern abstract methods
        protected abstract Task OpenInternal();

        protected virtual async Task WaitHandshake() {
        }

        protected abstract Task CloseInternal();

        protected abstract Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken);

        protected abstract Task SendBytesInternal(ReadOnlySpan<byte> bytes);

        // Template-pattern virtual methods
        protected virtual void OnOpening() {
        }

        protected virtual void OnOpened() {
        }

        protected virtual void OnClosing() {
        }

        protected virtual void OnClosed() {
        }

        protected virtual void OnReceivedBytes(byte[] bytes) {
        }

        protected virtual void OnSentBytes(ReadOnlySpan<byte> bytes) {
        }

        // Notify methods
        private void NotifyOpening() {
            OnOpening();
            Opening?.Invoke(this);
        }

        private void NotifyOpened() {
            OnOpened();
            Opened?.Invoke(this);
        }

        private void NotifyClosing() {
            OnClosing();
            Closing?.Invoke(this);
        }

        private void NotifyClosed() {
            OnClosed();
            Closed?.Invoke(this);
        }

        protected virtual void NotifyReceivedBytes(byte[] bytes) {
            OnReceivedBytes(bytes);
            ReceivedBytes?.Invoke(this, bytes.Length);
        }

        protected virtual void NotifySentBytes(ReadOnlySpan<byte> bytes) {
            OnSentBytes(bytes);
            SentBytes?.Invoke(this, bytes.Length);
        }

        private void CheckState(ProtocolChannelState expectedState) {
            if (State != expectedState)
                throw new InvalidOperationException($"Channel state was not {expectedState}");
        }

    }
    

}
