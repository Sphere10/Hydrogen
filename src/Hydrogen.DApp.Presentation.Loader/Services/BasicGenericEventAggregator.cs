using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.Services;

namespace Hydrogen.DApp.Presentation.Loader.Services
{
    public class BasicGenericEventAggregator : IGenericEventAggregator
    {
        private Dictionary<Type, List<Delegate>> Subscriptions { get; } = new ();
        
        
        public void Subscribe<T>(Action<T> handler)
        {
            if (Subscriptions.ContainsKey(typeof(T)))
            {
                Subscriptions[typeof(T)].Add(handler);
            }
            else
            {
                Subscriptions.Add(typeof(T), new List<Delegate> {handler});
            }
        }

        public void Unsubscribe<T>(Action<T> eventHandler)
        {
            if (Subscriptions.ContainsKey(typeof(T)))
            {
                Subscriptions[typeof(T)].Remove(eventHandler);
            }
        }

        public async Task PublishAsync<T>(T data)
        {
            if (Subscriptions.ContainsKey(typeof(T)))
            {
                await Task.Run(() =>
                {
                    foreach (Delegate o in Subscriptions[typeof(T)])
                    {
                        if (o is Action<T> handler)
                        {
                            try
                            {
                                handler.Invoke(data);

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                    }
                });
            }
        }
    }
}