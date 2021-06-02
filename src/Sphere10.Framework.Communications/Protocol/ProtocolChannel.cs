using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {

	public abstract class ProtocolChannel : IAsyncDisposable {

        // Events
        public event EventHandlerEx<object> Opening;
        public event EventHandlerEx<object> Opened;
        public event EventHandlerEx<object> Closing;
        public event EventHandlerEx<object> Closed;
        public event EventHandlerEx<object, int> ReceivedBytes;
        public event EventHandlerEx<object, int> SentBytes;

        // Fields
        private readonly CancellationTokenSource _cancelReceiveLoop;
        private readonly Task _receiveLoop;


        // Constructors
        protected ProtocolChannel() {
	        _cancelReceiveLoop = new CancellationTokenSource();
            _receiveLoop = new Task (async () => await ReceiveLoop(_cancelReceiveLoop.Token));
            State = ProtocolChannelState.Closed;
        }

        #region Properties

        public ProtocolChannelState State { get; private set; }

        public abstract CommunicationRole LocalRole { get; }

        public abstract CommunicationRole Initiator { get; }

        #endregion

        #region Methods

        public async Task Open() {
            CheckState(ProtocolChannelState.Closed);
            NotifyOpening();
            await OpenInternal();
            _receiveLoop.Start();
            State = ProtocolChannelState.Open;
            NotifyOpened();
            await WaitHandshake();
        }

        public async Task Close() {
	        const int forceCancelReceiveLoopMS = 150;
            CheckState(ProtocolChannelState.Open);
            State = ProtocolChannelState.Closing;
            NotifyClosing();
            _cancelReceiveLoop.CancelAfter(forceCancelReceiveLoopMS);
            try {
                // Receive loop SHOULD immediately react to channel State being Closed.
                // But if it does not, then after some time (forceCancelReceiveLoopMS), it is forced closed.
                await _receiveLoop;
                if (State != ProtocolChannelState.Closed) {
	                State = ProtocolChannelState.Closed;
	                NotifyClosed();
                }
            } catch (TaskCanceledException) {
            }

            await CloseInternal();
            // Note: NotifyClosed() is called by receive loop termination
        }

        public async Task SendBytes(byte[] bytes) {
	        if (!IsConnectionAlive())
		        return;
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

        protected virtual async Task ReceiveLoop(CancellationToken cancelToken) {
	        try {
		        while (State == ProtocolChannelState.Open && IsConnectionAlive()) {
			        var bytes = await ReceiveBytesInternal(cancelToken);
                    if (bytes?.Length > 0)
						NotifyReceivedBytes(bytes);
		        }
	        } catch (OperationCanceledException) {
		        // Fail gracefuully
	        } catch (Exception error) {
		        throw;  // LOG?
	        }
	        State = ProtocolChannelState.Closed;
	        NotifyClosed();
        }

        protected abstract Task CloseInternal();

        protected abstract Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken);

        protected abstract Task SendBytesInternal(ReadOnlySpan<byte> bytes);

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

        protected virtual void OnReceivedBytes(byte[] bytes) {
        }

        protected virtual void OnSentBytes(ReadOnlySpan<byte> bytes) {
        }

        #endregion

        #region Notify methods

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

        #endregion

        #region Aux 

        protected void CheckState(ProtocolChannelState expectedState) {
            if (State != expectedState)
                throw new InvalidOperationException($"Channel state was not {expectedState}");
        }

        #endregion

    }


}
