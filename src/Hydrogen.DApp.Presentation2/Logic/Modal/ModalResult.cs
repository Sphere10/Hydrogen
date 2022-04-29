using System;
using Hydrogen.DApp.Presentation2.UI.Dialogs;

namespace Hydrogen.DApp.Presentation2.Logic.Modal {

    /// <summary>
    /// Modal result - the result of a modal interaction.
    /// </summary>
    public class ModalResult {
        /// <summary>
        /// OK modal result.
        /// </summary>
        public static ModalResult Ok { get; } = new(ModalResultType.Ok);

        /// <summary>
        /// OK modal result with data.
        /// </summary>
        /// <param name="data"> data</param>
        /// <returns> modal result</returns>
        public static ModalResult OkData<T>(T? data) => new(ModalResultType.Ok, data);

        /// <summary>
        /// Modal exited result.
        /// </summary>
        public static ModalResult Exit { get; } = new(ModalResultType.Exit);

        /// <summary>
        /// Modal cancelled result
        /// </summary>
        public static ModalResult Cancel { get; } = new(ModalResultType.Cancel);

        /// <summary>
        /// Gets the result type.
        /// </summary>
        public ModalResultType ResultType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModalResult"/> class.
        /// </summary>
        /// <param name="resultType"> result type</param>
        public ModalResult(ModalResultType resultType) {
            ResultType = resultType;
        }

        /// <summary>
        /// Modal result
        /// </summary>
        /// <param name="resultType"> result type</param>
        /// <param name="data"> result dat</param>
        public ModalResult(ModalResultType resultType, object? data) {
            ResultType = resultType;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Gets the result data.
        /// </summary>
        public object? Data { get; }

        /// <summary>
        /// Gets the data as T. 
        /// </summary>
        /// <typeparam name="T"> type</typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"> thrown if data is not t.</exception>
        public T GetData<T>() {
            if (Data is T data) {
                return data;
            } else {
                throw new InvalidOperationException($"Data is not of type {typeof(T)}, type is {Data?.GetType()}");
            }
        }
    }

}