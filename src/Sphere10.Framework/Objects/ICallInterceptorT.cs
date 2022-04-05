namespace Sphere10.Framework {

	public interface ICallInterceptor<TResult> : ICallInterceptor {
		void CancelAndReturn(TResult result);
		TResult InterceptedResult { get; }
	}

}