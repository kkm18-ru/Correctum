using Correctum.Abstractions;
using Correctum.ViewModels.Windows;
using Correctum.Views.Windows;
using NLog;
using System.Windows;

namespace Correctum.Services
{
    internal static class ReceiptActionResolver
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Resolve(Type actionType)
        {
            var typeResolver = typeof(ReceiptActionResolver);
            var methodResolve = typeResolver.GetMethod("ResolveAsync",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            methodResolve = methodResolve!.MakeGenericMethod(actionType);
            var task = (Task)methodResolve.Invoke(null, null)!;
            task.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task ResolveAsync<TReceiptAction>()
            where TReceiptAction : class, IReceiptAction<TReceiptAction>, new()
        {
            try
            {
                var args = CreateArguments<TReceiptAction>();
                if (args is null) return;
                StorageContext.CreateBackup();
                using (var storage = new StorageContext())
                {
                    await TReceiptAction.InternalExecuteAsync(storage.Receipts, args);
                    await storage.SaveChangesAsync();
                }
                ((MainWindowViewModel)App.Current.MainWindow.DataContext).UpdatePanel();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Во время выполнения действия \"{ActionName}\" произошла ошибка.", TReceiptAction.Name);
                MessageBox.Show(
                    "Необработаная ошибка во время выполнения действия.\n\nПодробная информация записана в лог.",
                    string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static TReceiptAction? CreateArguments<TReceiptAction>()
            where TReceiptAction : class, IReceiptAction<TReceiptAction>, new()
        {
            var args = new TReceiptAction();
            if (App.Current.TryFindResource(new DataTemplateKey(typeof(TReceiptAction))) is { })
            {
                if (!(new ActionWindow(args).ShowDialog() ?? false))
                    return null;
            }
            return args;
        }
    }
}
