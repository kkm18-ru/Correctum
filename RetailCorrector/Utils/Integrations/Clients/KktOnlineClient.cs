using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace RetailCorrector.Utils.Integrations.Clients;

internal sealed partial class KktOnlineClient() : ApiClient<KktOnlineClient>("https://kkt-online.nalog.ru/lkip.html")
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private KktOnlineResponse Check(string query, string serial, int model)
    {
        ThrowIfDisposed();
        var uri = $"?query={query}&factory_number={serial}&model_code={model:D4}";
        Logger.Trace("Отправка запроса: {Path} {SerialNumber} {Model}", query, serial, model);
        var resp = Client.Send(new HttpRequestMessage(HttpMethod.Get, uri));
        Logger.Trace("Прочтение контента ответа в режиме потока");
        using var stream = resp.Content.ReadAsStream();
        Logger.Trace("Десериализация контента...");
        return JsonSerializer.Deserialize<KktOnlineResponse>(stream);
    }

    public StatusCode CheckKkt(string serial, KktModel model) =>
        Check("/kkt/model/check", serial, (int)model).ToStatusCode();

    public StatusCode CheckDrive(string serial, DriveModel model) =>
        Check("/fn/model/check", serial, (int)model).ToStatusCode();

    public enum StatusCode : byte
    {
        /// <summary>
        /// ККТ и/или ФН зарегистрированы в ФНС
        /// </summary>
        Ok,
        /// <summary>
        /// ККТ и/или ФН не зарегистрированы в ФНС
        /// </summary>
        NeedRegister,
        /// <summary>
        /// Отсутствует запись о произведенном экземпляре ККТ и/или ФН
        /// </summary>
        NoCreated,
        /// <summary>
        /// ФН закрыт и не может быть использован
        /// </summary>
        Archived,

        /// <summary>
        /// ФНС вернула ошибку при обработке запроса
        /// </summary>
        Error = 255,
        /// <summary>
        /// Неизвестный ответа (Не удалось определить статус)
        /// </summary>
        Unknown = 254,
    }

    private readonly partial struct KktOnlineResponse
    {
        [JsonPropertyName("status")]
        public byte Success { get; init; }

        [JsonPropertyName("error")]
        public string? Error { get; init; }

        [JsonPropertyName("check_status")]
        public byte? CanRegister { get; init; }

        [JsonPropertyName("check_result")]
        public string? Details { get; init; }

        public readonly StatusCode ToStatusCode()
        {
            if (Success == 0) return StatusCode.Error;
            if (Details is null) return StatusCode.NeedRegister;
            if (NoCreatedRegex().IsMatch(Details)) return StatusCode.NoCreated;
            if (Details.StartsWith("Экземпляр с данным заводским номером уже используется.")) return StatusCode.Ok;
            if (Details.StartsWith("Экземпляр ФН с данным заводским номером закрыт.")) return StatusCode.Archived;
            return StatusCode.Unknown;
        }

        [GeneratedRegex(@"^Заводской номер \w+ отсутствует в реестре произведенных экземпляров \w+$", 0, 250)]
        private static partial Regex NoCreatedRegex();
    }
    
    public enum DriveModel
    {
        Unknown = 0000,
        
        In36_4 = 0032,
        In15_4 = 0031,
        In36_3 = 0027,
        In15_3 = 0026,
        In15_1M = 0021,
        Av15_3 = 0029,
        Av36_3 = 0028,
        In36_1M = 0022,
        Iz15_4 = 0033,
        Av36_4 = 0037,
        Av15_4 = 0036,
        Pr15_4 = 0030,
        Ev36_4 = 0035,
        Ev15_4 = 0034,
        Ev36_3 = 0024,
        Ev15_3 = 0023,
    }
    
    public enum KktModel
    {
        Unknown = 0000,

        #region OOO "АТОЛ"

        ATOL_92F = 0121,
        ATOL_150F = 0122,
        ATOL_Sigma_7F = 0161,
        ATOL_Sigma_8F = 0162,
        ATOL_1F = 0168,
        ATOL_27F = 0169,
        ATOL_22_v2_F = 0183,
        ATOL_PT_5F = 0196,
        ATOL_47FA = 0200,
        ATOL_27_FP7_F = 0202,
        ATOL_42FA = 0210,
        ATOL_55_v2_F = 0222,
        ATOL_STB_6F = 0223,
        ATOL_35F = 0226,
        ATOL_FPrint_22PTK = 0002,
        ATOL_77F = 0006,
        ATOL_30F = 0007,
        ATOL_11F = 0008,
        ATOL_90F = 0018,
        ATOL_55F = 0019,
        ATOL_52F = 0020,
        ATOL_25F = 0021,
        ATOL_42FS = 0062,
        ATOL_60F = 0073,
        ATOL_15F = 0074,
        ATOL_20F = 0103,
        ATOL_50F = 0104,
        ATOL_91F = 0110,

        #endregion
        
        #region OOO "КАЗНАЧЕЙ"
        
        KAZNACHEY_FA = 0057,
        
        #endregion
        
        #region OOO "АТМ АЛЬЯНС СОЛЮШИНС"
        
        ALLIANCE_20F = 0217,
        
        #endregion
    }
}