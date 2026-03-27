using Avalonia.Controls;
using Correctum.Configurations;
using Correctum.Extensions;

namespace Correctum.Views.Windows
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; } = null!;

        public MainWindow()
        {
            if (Instance is not null)
                throw new Exception();
            InitializeComponent();
            Instance = this;
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            Configuration.Save();
            base.OnClosing(e);
        }
    }
}