using System;
using System.Threading.Tasks;

namespace Sphere10.Hydrogen.Presentation.Services {
    public interface IGenericEventAggregator {
        void Subscribe<T>(Action<T> handler);

        void Unsubscribe<T>(Action<T> eventHandler);

        Task PublishAsync<T>(T data);
    }
}