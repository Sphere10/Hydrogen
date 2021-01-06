namespace VelocityNET.Presentation.Hydrogen.Components.Modal
{
    public class ModalResult
    {
    }

    public class ModalResult<T> : ModalResult
    {
        public ModalResult(T data)
        {
            Data = data;
        }
        
        private T Data { get; set; }
    }
}