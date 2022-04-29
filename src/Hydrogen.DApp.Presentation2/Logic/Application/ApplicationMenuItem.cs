using Hydrogen;

namespace Hydrogen.DApp.Presentation2.Logic
{

    public abstract class ApplicationMenuItem : IApplicationMenuItem
    {
        public event EventHandlerEx Hover;
        public event EventHandlerEx Select;

        public string Icon { get; init; }

        public string Title { get; init; }

        protected virtual void OnHover()
        {
        }

        protected virtual void OnSelect()
        {
        }


        internal void NotifyHover()
        {
            Hover?.Invoke();
            OnHover();
        }

        internal void NotifySelect()
        {
            Select?.Invoke();
            OnSelect();
        }
    }

}