﻿using System;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Saga {
	public abstract class Saga {
		private IBus _bus;

		protected ISagaData SagaDataBase { get; set; }

		public IBus Bus {
			get => _bus ?? throw new InvalidOperationException("Cannot proceed! IBus not instantiated. Do NOT use your own bus here!");
			set => _bus = value;
		}

		public bool Completed { get; private set; }

		protected Saga(IBus bus) {
			_bus = bus;
		}

		protected internal abstract void ConfigureHowToFindSaga(IFindSaga sagaFindMap);

		private void VerifySagaCanHandleTimeout<T>(T timeoutMessage) where T : IMessage {
			if (!(timeoutMessage is IHandleTimeout<T>)) {
				throw new Exception(
					$"'{(object)GetType().Name}' Cannot proceed! TimeoutManager for '{(object)timeoutMessage}' must implement 'IHandleTimeouts<{(object)typeof(T).FullName}>'");
			}
		}

		protected void RequestTimeout<T>(DateTime at, IMessage timeoutMessage) where T : IMessage {
			if (at.Kind == DateTimeKind.Unspecified) throw new InvalidOperationException("The DateTime 'at' must be specified: Local, UTC etc. Cannot Proceed!");

			VerifySagaCanHandleTimeout(timeoutMessage);

			Bus.RegisterTimeout(at, timeoutMessage);
		}

		protected void RequestTimeout<T>(TimeSpan within, T timeoutMessage) where T : IMessage {
			VerifySagaCanHandleTimeout(timeoutMessage);

			Bus.RegisterTimeout(within, timeoutMessage);
		}

		protected virtual void ReplyToOriginator(IMessage message) {
			if (string.IsNullOrEmpty(this.SagaDataBase.Originator))
				throw new Exception("Cannot proceed! SagaDataBase.Originator is null.");

			//BusSetup.SendAndForget(SagaDataBase.Originator, message, new NotI{} as IMessageHeader );
		}

		protected void MarkAsComplete() => Completed = true;
	}
}
