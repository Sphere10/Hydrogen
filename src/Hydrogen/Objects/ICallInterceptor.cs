namespace Sphere10.Framework {

	public interface ICallInterceptor {
		CallInterceptOption Option { get; }
		void CancelAndThrow();
		void CancelAndReturnDefault();
	}

}