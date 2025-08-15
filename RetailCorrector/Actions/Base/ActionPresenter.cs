namespace RetailCorrector.Actions
{
    internal abstract class ActionPresenter<TP, TV> : IActionPresenter
        where TV : ActionView<TV, TP>, new()
        where TP : ActionPresenter<TP, TV>, new()
    {
        public static void Show() => new ActionWindow(new TV()).ShowDialog();
        public abstract void Start();
    }

    internal abstract class ActionPresenter<T> : IActionPresenter
        where T : ActionPresenter<T>, new()
    {
        public static void Show() => new ActionWindow(new T()).ShowDialog();
        public abstract void Start();
    }

    internal interface IActionPresenter
    {
        void Start();
    }
}
