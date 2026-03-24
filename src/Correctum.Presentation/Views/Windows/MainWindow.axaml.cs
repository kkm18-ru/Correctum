using Avalonia.Controls;
using Correctum.Configurations;
using Correctum.Extensions;

namespace Correctum.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            Configuration.Save();
            base.OnClosing(e);
        }
    }
}