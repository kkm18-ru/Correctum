using MessageDialog;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            if (Storage.Instance == null!)
            {
                var createBase = () =>
                {
                    var dialog = new SaveFileDialog
                    {
                        Filter = "База данных Correctum|*.cdb",
                        AddExtension = true,
                    };
                    if (!dialog.ShowDialog() ?? false)
                    {
                        MessageBox.Show("Путь к базе данных не выбран. Приложение закрывается.");
                        Close();
                    }
                    Storage.Create(dialog.FileName);
                };
                var loadBase = () =>
                {
                    var dialog = new OpenFileDialog
                    {
                        Filter = "База данных Correctum|*.cdb",
                    };
                    if (!dialog.ShowDialog() ?? false)
                    {
                        MessageBox.Show("Путь к базе данных не выбран. Приложение закрывается.");
                        Close();
                    }
                    Storage.Load(dialog.FileName);
                };
                var options = new MessageDialogOptions
                {
                    Title = "Хранилище не проинициализировано!",
                    Message = "Для инициализации хранилища необходимо открыть существующую базу Correctum или создать новую.",
                    Buttons = [
                        new MessageDialogButtonOptions("Создать", createBase),
                    new MessageDialogButtonOptions("Загрузить", loadBase),
                    new MessageDialogButtonOptions("Закрыть", Close, MessageDialogButtonType.Cancel)
                        ]
                };
                new MessageDialog.MessageDialog(this, options).ShowDialog();
            }
            base.OnContentRendered(e);
        }
    }
}
