namespace Hydrogen {

	public interface ICallInterceptor {
		CallInterceptOption Option { get; }
		void CancelAndThrow();
		void CancelAndReturnDefault();
	}

}