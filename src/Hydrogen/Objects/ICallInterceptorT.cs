namespace Hydrogen {

	public interface ICallInterceptor<TResult> : ICallInterceptor {
		void CancelAndReturn(TResult result);
		TResult InterceptedResult { get; }
	}

}