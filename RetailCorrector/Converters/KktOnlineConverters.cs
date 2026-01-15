using static RetailCorrector.Utils.Integrations.Clients.KktOnlineClient;

namespace RetailCorrector.Converters;

internal static class KktOnlineConverters
{
    public static KktModel FromAtolKktModel(uint model) => model switch
    {
        57 => KktModel.ATOL_25F,
        61 => KktModel.ATOL_30F,
        62 => KktModel.ATOL_55F,
        63 => KktModel.ATOL_FPrint_22PTK,
        64 => KktModel.ATOL_52F,
        67 => KktModel.ATOL_11F,
        69 => KktModel.ATOL_77F,
        72 => KktModel.ATOL_90F,
        75 => KktModel.ATOL_60F,
        77 => KktModel.ATOL_42FS,
        78 => KktModel.ATOL_15F,
        80 => KktModel.ATOL_50F,
        81 => KktModel.ATOL_20F,
        82 => KktModel.ATOL_91F,
        84 => KktModel.ATOL_92F,
        87 => KktModel.ATOL_27F,
        90 => KktModel.ATOL_Sigma_7F,
        91 => KktModel.ATOL_Sigma_8F,
        93 => KktModel.ATOL_1F,
        76 => KktModel.KAZNACHEY_FA,
        95 => KktModel.ATOL_22_v2_F,
        48 => KktModel.ATOL_47FA,
        89 => KktModel.ATOL_PT_5F,
        99 => KktModel.ATOL_27_FP7_F,
        70 => KktModel.ATOL_42FA,
        50 => KktModel.ALLIANCE_20F,
        66 => KktModel.ATOL_55_v2_F,
        92 => KktModel.ATOL_STB_6F,
        97 => KktModel.ATOL_35F,
        _ => KktModel.Unknown
    };

    public static DriveModel FromAtolFnModel(string model) => model switch
    {
        _ => DriveModel.Unknown
    };
}