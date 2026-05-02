using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Correctum.Utils;

public static class SecretHelper
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static string EncodeSecret(string rawSecret, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrWhiteSpace(rawSecret))
        {
            Logger.Info("Шифрование секрета не требуется: секрет - пустая строка.");
            return string.Empty;
        }

        try
        {
            Span<byte> rawBytes = stackalloc byte[encoding.GetMaxByteCount(rawSecret.Length)];
            var length = encoding.GetBytes(rawSecret, rawBytes);
            Logger.Trace("Строка {EncodedPassword} преобразована в массив с размером {Length}", rawSecret, length);
            Span<byte> bytes = stackalloc byte[2048];
            if (!ProtectedData.TryProtect(rawBytes[..length], DataProtectionScope.LocalMachine, bytes, out length))
            {
                Logger.Error("Не удалось выполнить переобразование байт. (Код ошибки: {LastError})", Marshal.GetLastPInvokeError());
                return string.Empty;
            }
            Logger.Trace("Выполнено преобразование байт (сокрытие секрета).");
            Span<char> chars = stackalloc char[length / 3 * 5];
            Convert.TryToBase64Chars(bytes[..length], chars, out _);
            Logger.Trace("Выполнен перевод массива байт в Base64-формат.");
            var encoded = new string(chars.TrimEnd('\0'));
            Logger.Info("Шифрование секрета завершено успешно.");
            Logger.Debug("Результат шифрования пароля \"{DecodedPassword}\": {EncodedPassword}", rawSecret, encoded);
            return encoded;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Не удалось выполнить шифрование секрета.");
            return string.Empty;
        }
    }

    public static string DecodeSecret(string encoded, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrWhiteSpace(encoded))
        {
            Logger.Info("Расшифровывание секрета не требуется: шифр - пустая строка.");
            return string.Empty;
        }

        try
        {
            Span<byte> encodedBytes = stackalloc byte[encoding.GetMaxByteCount(encoded.Length)];
            Convert.TryFromBase64String(encoded, encodedBytes, out var len);
            Logger.Trace("Выполнено получение массива байт из Base64-формат.");
            Span<byte> bytes = stackalloc byte[2048];
            if (!ProtectedData.TryUnprotect(encodedBytes[..len], DataProtectionScope.LocalMachine, bytes, out len))   
            {
                Logger.Error("Не удалось выполнить переобразование байт. (Код ошибки: {LastError})", Marshal.GetLastPInvokeError());
                return string.Empty;
            }
            Logger.Trace("Выполнено преобразование байт (восстановление секрета).");
            Span<char> chars = stackalloc char[encoding.GetMaxCharCount(len)];
            len = encoding.GetChars(bytes[..len], chars);
            Logger.Trace("Массив размером {Length} преобразован в строку", len);
            var rawSecret = new string(chars.TrimEnd('\0'));
            Logger.Info("Расшифровывание секрета завершено успешно.");
            Logger.Debug("Результат расшифровки \"{EncodedPassword}\": {DecodedPassword}", encoded, rawSecret);
            return rawSecret;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Не удалось выполнить расшифровывание секрета.");
            return string.Empty;
        }
    }
}