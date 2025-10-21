using Kraig;
using RetailCorrector.UI.Windows;

namespace RetailCorrector.Actions
{
    internal abstract class ActionBase<TP, TV> : IAction
        where TV : ActionView<TV, TP>, new()
        where TP : ActionBase<TP, TV>, ISingleton<TP>
    {
        public abstract string Name { get; }
        public void Show() => new ActionWindow(new TV()).ShowDialog();
        public abstract void Start();
    }

    internal abstract class ActionBase<T> : IAction
        where T : ActionBase<T>, ISingleton<T>
    {
        public abstract string Name { get; }
        public void Show() => new ActionWindow(T.Instance).ShowDialog();
        public abstract void Start();
    }

    internal interface IAction
    {
        string Name { get; }
        void Show();
        void Start();
    }
}
