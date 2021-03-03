using System;
using System.Threading.Tasks;

namespace Sphere10.Helium.Bus
{
    public interface ICallback
    {
        Task<T> Register<T>();

        Task<T> Register<T>(Func<CompletionResult, T> completion);

        Task Register(Action<CompletionResult> completion);

        IAsyncResult Register(AsyncCallback callback, object state);

        void Register<T>(Action<T> callback);

        void Register<T>(Action<T> callback, object synchronizer);
    }
}
