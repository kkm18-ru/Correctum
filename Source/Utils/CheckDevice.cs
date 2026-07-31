using Correctum.Enums;
using Correctum.Utils.Integrations;
using Correctum.Utils.Integrations.Clients;

namespace Correctum.Utils;

internal static class CheckDevice
{
    private static readonly KktOnlineClient KktOnline = new();
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static AtolKktProblems SearchProblemsInKkt(FptrHandle fptr) =>
        CheckFnsKkt(fptr) | CheckFnsFn(fptr) | CheckLicenses(fptr);

    private static AtolKktProblems CheckFnsKkt(FptrHandle fptr)
    {
        Logger.Trace("Получение модели ККТ...");
        var model = KktOnlineHelper.FromAtolKktModel(fptr.Model);
        Logger.Debug("Модель ККТ: {AtolModel} > {KktOnlineModel}", fptr.Model, model.ToString("G"));
        if (model == 0) return AtolKktProblems.ModelKktUnknown;
        Logger.Trace("Проверка статуса регистрации ККТ...");
        return KktOnline.CheckKkt(fptr.SerialNumber, model) switch
        {
            KktOnlineClient.StatusCode.Ok => AtolKktProblems.None,
            KktOnlineClient.StatusCode.Error => AtolKktProblems.CheckKktError,
            KktOnlineClient.StatusCode.NeedRegister => AtolKktProblems.CheckKktNeedRegister,
            KktOnlineClient.StatusCode.NoCreated => AtolKktProblems.CheckKktNoCreated,
            _ => AtolKktProblems.CheckKktUnknown
        };
    }

    private static AtolKktProblems CheckFnsFn(FptrHandle fptr)
    {
        Logger.Trace("Получение исполнения ФН...");
        var model = KktOnlineHelper.FromAtolFnModel(fptr.FnModel);
        Logger.Debug("Исполнение ФН: {AtolModel} > {KktOnlineModel}", fptr.FnModel, model.ToString("G"));
        if (model == 0) return AtolKktProblems.ModelFnUnknown;
        Logger.Trace("Проверка статуса регистрации ФН...");
        return KktOnline.CheckDrive(fptr.FnSerialNumber, model) switch
        {
            KktOnlineClient.StatusCode.Ok => AtolKktProblems.None,
            KktOnlineClient.StatusCode.Error => AtolKktProblems.CheckFnError,
            KktOnlineClient.StatusCode.NeedRegister => AtolKktProblems.CheckFnNeedRegister,
            KktOnlineClient.StatusCode.NoCreated => AtolKktProblems.CheckFnNoCreated,
            KktOnlineClient.StatusCode.Archived => AtolKktProblems.CheckFnArchived,
            _ => AtolKktProblems.CheckFnUnknown
        };
    }

    private static AtolKktProblems CheckLicenses(FptrHandle fptr)
    {
        const uint printLicenseNumber = 17;
        var problems = AtolKktProblems.None;

        if (fptr.ConfigurationVersion < new Version(5, 12))
        {
            Logger.Warn("Конфигурационный модуль устарел: {Version} (Минимальная версия: 5.12.0)", fptr.ConfigurationVersion);
            problems |= AtolKktProblems.ConfigurationOutdated; 
        }
        
        var license = fptr.GetLicenseInfo(printLicenseNumber);

        if (!(license?.Entered ?? false))
        {
            Logger.Warn("Лицензия №{LicenseNumber} не введена", printLicenseNumber);
            problems |= AtolKktProblems.AdditionalPrintNoFound;
        }

        var expiredDate = license?.ValidUntil ?? DateTime.MinValue;
        if (expiredDate <= DateTime.Today)
        {
            Logger.Warn("Срок действия лицензии №{LicenseNumber} истек: {ExpiredDate}", printLicenseNumber, expiredDate);
            problems |= AtolKktProblems.AdditionalPrintExpired;
        }
        
        return problems;
    }
}

file static class KktOnlineHelper
{
    public static KktOnlineClient.KktModel FromAtolKktModel(uint model) => model switch
    {
        57 => KktOnlineClient.KktModel.ATOL_25F,
        61 => KktOnlineClient.KktModel.ATOL_30F,
        62 => KktOnlineClient.KktModel.ATOL_55F,
        63 => KktOnlineClient.KktModel.ATOL_FPrint_22PTK,
        64 => KktOnlineClient.KktModel.ATOL_52F,
        67 => KktOnlineClient.KktModel.ATOL_11F,
        69 => KktOnlineClient.KktModel.ATOL_77F,
        72 => KktOnlineClient.KktModel.ATOL_90F,
        75 => KktOnlineClient.KktModel.ATOL_60F,
        77 => KktOnlineClient.KktModel.ATOL_42FS,
        78 => KktOnlineClient.KktModel.ATOL_15F,
        80 => KktOnlineClient.KktModel.ATOL_50F,
        81 => KktOnlineClient.KktModel.ATOL_20F,
        82 => KktOnlineClient.KktModel.ATOL_91F,
        84 => KktOnlineClient.KktModel.ATOL_92F,
        87 => KktOnlineClient.KktModel.ATOL_27F,
        90 => KktOnlineClient.KktModel.ATOL_Sigma_7F,
        91 => KktOnlineClient.KktModel.ATOL_Sigma_8F,
        93 => KktOnlineClient.KktModel.ATOL_1F,
        76 => KktOnlineClient.KktModel.KAZNACHEY_FA,
        95 => KktOnlineClient.KktModel.ATOL_22_v2_F,
        48 => KktOnlineClient.KktModel.ATOL_47FA,
        89 => KktOnlineClient.KktModel.ATOL_PT_5F,
        99 => KktOnlineClient.KktModel.ATOL_27_FP7_F,
        70 => KktOnlineClient.KktModel.ATOL_42FA,
        50 => KktOnlineClient.KktModel.ALLIANCE_20F,
        66 => KktOnlineClient.KktModel.ATOL_55_v2_F,
        92 => KktOnlineClient.KktModel.ATOL_STB_6F,
        97 => KktOnlineClient.KktModel.ATOL_35F,
        _ => KktOnlineClient.KktModel.Unknown
    };

    public static KktOnlineClient.DriveModel FromAtolFnModel(string model) => model switch
    {
        "ФН-1.2 исполнение Ин36-4" => KktOnlineClient.DriveModel.In36_4, //0032
        "ФН-1.2 исполнение Ин15-4" => KktOnlineClient.DriveModel.In15_4, //0031
        "ФН-1.2 исполнение Ин36-3" => KktOnlineClient.DriveModel.In36_3, //0027
        "ФН-1.2 исполнение Ин15-3" => KktOnlineClient.DriveModel.In15_3, //0026
        "ФН-1.1М исполнение Ин15-1М" => KktOnlineClient.DriveModel.In15_1M, //0021
        "ФН-1.2 исполнение Ав15-3" => KktOnlineClient.DriveModel.Av15_3, //0029
        "ФН-1.2 исполнение Ав36-3" => KktOnlineClient.DriveModel.Av36_3, //0028
        "ФН-1.1М исполнение Ин36-1М" => KktOnlineClient.DriveModel.In36_1M, //0022
        "ФН-1.2 исполнение Из15-4" => KktOnlineClient.DriveModel.Iz15_4, //0033
        "ФН-1.2 исполнение Ав36-4" => KktOnlineClient.DriveModel.Av36_4, //0037
        "ФН-1.2 исполнение Ав15-4" => KktOnlineClient.DriveModel.Av15_4, //0036
        "ФН-1.2 исполнение Пр15-4" => KktOnlineClient.DriveModel.Pr15_4, //0030
        "ФН-1.2 исполнение Эв36-4" => KktOnlineClient.DriveModel.Ev36_4, //0035
        "ФН-1.2 исполнение Эв15-4" => KktOnlineClient.DriveModel.Ev15_4, //0034
        "ФН-1.2 исполнение Эв36-3" => KktOnlineClient.DriveModel.Ev36_3, //0024
        "ФН-1.2 исполнение Эв15-3" => KktOnlineClient.DriveModel.Ev15_3, //0023
        _ => KktOnlineClient.DriveModel.Unknown
    };
}