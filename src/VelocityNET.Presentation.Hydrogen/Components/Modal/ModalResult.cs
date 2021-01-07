using System;

namespace VelocityNET.Presentation.Hydrogen.Components.Modal
{

    public class ModalResult
    {
        public static ModalResult Ok { get; } = new (ModalResultType.Ok);
        
        public static ModalResult OkData(object data) => new (ModalResultType.Ok, data);
        
        public static ModalResult Exit { get; } = new (ModalResultType.Exit);
        
        public static ModalResult Cancel { get; } = new (ModalResultType.Cancel);

        public ModalResultType ResultType { get; }

        public ModalResult(ModalResultType resultType)
        {
            ResultType = resultType;
        }

        public ModalResult(ModalResultType resultType, object data)
        {
            ResultType = resultType;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public object? Data { get; }

        /// <summary>
        /// Gets the data as T. 
        /// </summary>
        /// <typeparam name="T"> type</typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"> thrown if data is not t.</exception>
        public T GetData<T>()
        {
            if (Data is T data)
            {
                return data;
            }
            else
            {
                throw new InvalidOperationException($"Data is not of type {typeof(T)}, type is {Data.GetType()}");
            }
        }
    }

}