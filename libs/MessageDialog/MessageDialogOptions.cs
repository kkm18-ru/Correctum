using System.ComponentModel.DataAnnotations;

namespace MessageDialog
{
    public enum MessageDialogButtonType { Normal, Cancel, Default }
    public readonly record struct MessageDialogButtonOptions(
        string Text, 
        Action Action, 
        MessageDialogButtonType Type = MessageDialogButtonType.Normal);

    public sealed class MessageDialogOptions
    {
        public IList<MessageDialogButtonOptions> Buttons { get; init; } = [];
        public string? Message { get; set; }
        public string? Title { get; set; }

        internal static void Validate(MessageDialogOptions options)
        {
            ValidateButtons(options.Buttons);
        }

        private static void ValidateButtons(IList<MessageDialogButtonOptions> buttons)
        {
            if (buttons.Count == 0)
                throw new ValidationException("Отсутствуют кнопки диалогового окна.");

            for (var i = 0; i < buttons.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(buttons[i].Text))
                    throw new ValidationException($"Некорректная настройка кнопки под индексом {i}.",
                        new ValidationException("Текст кнопки пуст."));
            }

            if (buttons.Count(b => b.Type == MessageDialogButtonType.Cancel) > 1)
                throw new ValidationException($"Тип {nameof(MessageDialogButtonType.Cancel)} встречается более одного раза.");
            if (buttons.Count(b => b.Type == MessageDialogButtonType.Default) > 1)
                throw new ValidationException($"Тип {nameof(MessageDialogButtonType.Default)} встречается более одного раза.");
            
            var duplicateButtonText = buttons
                .GroupBy(b => b.Text, (text, btns) => (text, btns.Count(b => b.Text == text)))
                .Where(i => i.Item2 > 1).Select(i => (string?)i.text).FirstOrDefault();
            if (duplicateButtonText is { } duple)
                throw new ValidationException($"Текст \"{duple}\" встречается более одного раза.");
        }
    }
}
