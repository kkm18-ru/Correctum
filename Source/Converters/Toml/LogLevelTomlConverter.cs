using NLog;
using Tomlyn.Serialization;

namespace Correctum.Converters.Toml;

internal sealed class LogLevelTomlConverter : TomlConverter<LogLevel>
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public override LogLevel? Read(TomlReader reader)
    {
        try
        {
            return LogLevel.FromString(reader.GetString());
        }
        catch (ArgumentException e) when (e.ParamName == "levelName")
        {
            logger.Error(e, 
                "Некорректный формат уровня логирования (Значение: {Value}). " +
                "Ожидаемые значения: Trace, Debug, Info, Warn, Error, Fatal, Off.", 
                reader.GetRawText());
            return null;
        }
    }

    public override void Write(TomlWriter writer, LogLevel value) =>
        writer.WriteStringValue(value.Name);
}
