using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace RetailCorrector.Exceptions.Saby;

public class SabyException : Exception
{
    public string Caption { get; init; }
    public string MessageText { get; init; }
    public MessageBoxImage Icon { get; init; }
    
    public SabyException(Stream stream)
    {
        var obj = JsonSerializer.Deserialize<Response>(stream);
        Caption = $"Ошибка {obj.Error.Code}";
        MessageText = obj.Error.Text;
        Icon = obj.Error.Type switch
        {
            "warning" => MessageBoxImage.Warning,
            "error" => MessageBoxImage.Error,
            _ => MessageBoxImage.None
        };
    } 
}

file struct Response
{
    [JsonPropertyName("error")]
    public Error Error { get; set; }
}

file struct Error
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
    
    [JsonPropertyName("details")]
    public string Text { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }
}