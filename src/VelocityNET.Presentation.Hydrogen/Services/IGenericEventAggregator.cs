using System;
using System.Threading.Tasks;

namespace VelocityNET.Presentation.Hydrogen.Services
{
    public interface IGenericEventAggregator
    {
        void Subscribe<T>(Action<T> handler);

        void Unsubscribe<T>(Action<T> eventHandler);

        Task PublishAsync<T>(T data);
    }
}