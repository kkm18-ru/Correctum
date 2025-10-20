using RetailCorrector.UI.Windows;

namespace RetailCorrector.Actions
{
    internal abstract class ActionBase<TP, TV> : IAction
        where TV : ActionView<TV, TP>, new()
        where TP : ActionBase<TP, TV>, new()
    {
        public abstract string Name { get; }
        public static void Show() => new ActionWindow(new TV()).ShowDialog();
        public abstract void Start();
    }

    internal abstract class ActionBase<T> : IAction
        where T : ActionBase<T>, new()
    {
        public abstract string Name { get; }
        public static void Show() => new ActionWindow(new T()).ShowDialog();
        public abstract void Start();
    }

    internal interface IAction
    {
        string Name { get; }
        static abstract void Show();
        void Start();
    }
}
