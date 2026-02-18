using RetailCorrector.Converters;
using RetailCorrector.Enums;
using RetailCorrector.Utils.Integrations;
using RetailCorrector.Utils.Integrations.Clients;

namespace RetailCorrector.Utils;

internal static class CheckDevice
{
    private static readonly KktOnlineClient KktOnline = new();
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static AtolKktProblems SearchProblemsInKkt(FptrHandle fptr) =>
        CheckFnsKkt(fptr) | CheckFnsFn(fptr) | CheckLicenses(fptr);

    private static AtolKktProblems CheckFnsKkt(FptrHandle fptr)
    {
        Logger.Trace("Получение модели ККТ...");
        var model = KktOnlineConverters.FromAtolKktModel(fptr.Model);
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
        var model = KktOnlineConverters.FromAtolFnModel(fptr.FnModel);
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