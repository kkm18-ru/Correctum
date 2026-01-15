using RetailCorrector.Converters;
using RetailCorrector.Enums;
using RetailCorrector.Utils.Integrations;
using RetailCorrector.Utils.Integrations.Clients;

namespace RetailCorrector.Utils;

internal static class CheckDevice
{
    private static readonly KktOnlineClient KktOnline = new();

    public static AtolKktProblems SearchProblemsInKkt(FptrHandle fptr) =>
        CheckFnsKkt(fptr) | CheckFnsFn(fptr) | CheckLicenses(fptr);

    private static AtolKktProblems CheckFnsKkt(FptrHandle fptr)
    {
        var model = KktOnlineConverters.FromAtolKktModel(fptr.Model);
        if (model == 0) return AtolKktProblems.ModelKktUnknown;
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
        var model = KktOnlineConverters.FromAtolFnModel(fptr.FnModel);
        if (model == 0) return AtolKktProblems.ModelFnUnknown;
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
            problems |= AtolKktProblems.ConfigurationOutdated; 
        
        var license = fptr.GetLicenseInfo(printLicenseNumber);
        
        if (!(license?.Entered ?? false))
            problems |= AtolKktProblems.AdditionalPrintNoFound;
        
        if ((license?.ValidUntil ?? DateTime.MinValue) <= DateTime.Today)
            problems |= AtolKktProblems.AdditionalPrintExpired;
        
        return problems;
    }
}