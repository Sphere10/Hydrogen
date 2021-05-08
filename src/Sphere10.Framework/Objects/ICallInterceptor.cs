namespace Sphere10.Framework {

	public interface IInterceptor {
		InterceptOption Option { get; }
		void CancelAndThrow();
		void CancelAndReturnDefault();
	}

	public interface IInterceptor<TResult> : IInterceptor {
		void CancelAndReturn(TResult result);
		TResult InterceptedResult { get; }
	}

}