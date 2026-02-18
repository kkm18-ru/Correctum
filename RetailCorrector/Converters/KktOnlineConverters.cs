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
        "ФН-1.2 исполнение Ин36-4" => DriveModel.In36_4, //0032
        "ФН-1.2 исполнение Ин15-4" => DriveModel.In15_4, //0031
        "ФН-1.2 исполнение Ин36-3" => DriveModel.In36_3, //0027
        "ФН-1.2 исполнение Ин15-3" => DriveModel.In15_3, //0026
        "ФН-1.1М исполнение Ин15-1М" => DriveModel.In15_1M, //0021
        "ФН-1.2 исполнение Ав15-3" => DriveModel.Av15_3, //0029
        "ФН-1.2 исполнение Ав36-3" => DriveModel.Av36_3, //0028
        "ФН-1.1М исполнение Ин36-1М" => DriveModel.In36_1M, //0022
        "ФН-1.2 исполнение Из15-4" => DriveModel.Iz15_4, //0033
        "ФН-1.2 исполнение Ав36-4" => DriveModel.Av36_4, //0037
        "ФН-1.2 исполнение Ав15-4" => DriveModel.Av15_4, //0036
        "ФН-1.2 исполнение Пр15-4" => DriveModel.Pr15_4, //0030
        "ФН-1.2 исполнение Эв36-4" => DriveModel.Ev36_4, //0035
        "ФН-1.2 исполнение Эв15-4" => DriveModel.Ev15_4, //0034
        "ФН-1.2 исполнение Эв36-3" => DriveModel.Ev36_3, //0024
        "ФН-1.2 исполнение Эв15-3" => DriveModel.Ev15_3, //0023
        _ => DriveModel.Unknown
    };
}