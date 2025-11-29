using RetailCorrector.UI.Windows;
using RetailCorrector.Utils;

namespace RetailCorrector.Actions
{
    internal abstract class ActionBase<TP, TV> : IAction, ISingleton<TP>
        where TV : ActionView<TV, TP>, new()
        where TP : ActionBase<TP, TV>, ISingleton<TP>, new()
    {
        public static TP Instance => field ??= new TP();

        public abstract string Name { get; }
        public void Show() => new ActionWindow(new TV()).ShowDialog();
        public abstract void Start();
    }

    internal abstract class ActionBase<T> : IAction, ISingleton<T>
        where T : ActionBase<T>, ISingleton<T>, new()
    {
        public static T Instance => field ??= new T();

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
