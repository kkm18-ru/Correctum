using System.Windows;
using System.Windows.Controls;

namespace Correctum.UI.Controls
{
    internal class NotifyTextBox : TextBox
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationError));
        }

        private void OnValidationError(object? sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                MessageBox.Show(e.Error.ErrorContent?.ToString(), "", 0, MessageBoxImage.Error);
        }
    }
}
