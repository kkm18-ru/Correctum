using RetailCorrector.UI;

namespace RetailCorrector.Actions
{
    internal abstract class ActionBase<TP, TV> : IAction
        where TV : ActionView<TV, TP>, new()
        where TP : ActionBase<TP, TV>, new()
    {
        public static void Show() => new ActionWindow(new TV()).ShowDialog();
        public abstract void Start();
    }

    internal abstract class ActionBase<T> : IAction
        where T : ActionBase<T>, new()
    {
        public static void Show() => new ActionWindow(new T()).ShowDialog();
        public abstract void Start();
    }

    internal interface IAction
    {
        void Start();
    }
}
