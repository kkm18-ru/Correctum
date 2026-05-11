using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Correctum.Fptr;

public sealed class FptrLibrary
{
    public static FptrLibrary Instance { get; private set; } = null!;

    internal FptrHandleManagement Create { get; private set; }
    internal FptrHandleManagement Destroy { get; private set; }
    internal FptrAction Registration { get; private set; }
    internal FptrAction OpenReceipt { get; private set; }
    internal FptrAction UtilFormTLV { get; private set; }
    internal FptrAction FnQueryData { get; private set; }
    internal FptrAction QueryData { get; private set; }
    internal FptrAction Report { get; private set; }
    internal FptrAction ErrorCode { get; private set; }
    internal FptrAction Open { get; private set; }
    internal FptrAction Close { get; private set; }
    internal FptrAction OperatorLogin { get; private set; }
    internal FptrAction CloseReceipt { get; private set; }
    internal FptrAction CancelReceipt { get; private set; }
    internal FptrAction Payment { get; private set; }
    internal FptrAction ReceiptTotal { get; private set; }
    internal FptrAction OpenShift { get; private set; }
    internal FptrGetBoolean GetParamBool { get; private set; }
    internal FptrGetInteger GetParamInt { get; private set; }
    private FptrGetByteArray GetParamByteArray { get; set; }
    private FptrGetByteArray GetParamStr { get; set; }
    private FptrGetDate GetParamDate { get; set; }
    internal FptrSetBoolean SetParamBool { get; private set; }
    internal FptrSetInteger SetParamInt { get; private set; }
    internal FptrSetDouble SetParamDouble { get; private set; }
    internal FptrSetByteArray SetParamByteArray { get; private set; }
    private FptrSetString SetParamStr { get; set; }
    private FptrSetDate SetParamDate { get; set; }
    internal FptrShowProperties ShowProperties { get; private set; }
    internal FptrErrorDescription ErrorDescription { get; private set; }
    internal FptrGetSetting GetSingleSetting { get; private set; }
    internal FptrGetFlag IsOpened { get; private set; }

    internal static Encoding Encoding
    {
        get
        {
            if (OperatingSystem.IsWindows()) return Encoding.Unicode;
            if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() ||
                OperatingSystem.IsMacCatalyst()) return Encoding.UTF32;
            throw new PlatformNotSupportedException();
        }
    }

    internal static int DefaultSizeBytes
    {
        get
        {
            if (OperatingSystem.IsWindows()) return 128;
            if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() ||
                OperatingSystem.IsMacCatalyst()) return 256;
            throw new PlatformNotSupportedException();
        }
    }

    private readonly List<Fptr> handles = [];
    private readonly nint hLibrary;

    private FptrLibrary()
    {
        hLibrary = NativeLibrary.Load("fptr10.dll");
        Create = GetOfLib<FptrHandleManagement>("libfptr_create");
        Destroy = GetOfLib<FptrHandleManagement>("libfptr_destroy");
        Registration = GetOfLib<FptrAction>("libfptr_registration");
        OpenReceipt = GetOfLib<FptrAction>("libfptr_open_receipt");
        UtilFormTLV = GetOfLib<FptrAction>("libfptr_util_form_tlv");
        FnQueryData = GetOfLib<FptrAction>("libfptr_fn_query_data");
        QueryData = GetOfLib<FptrAction>("libfptr_query_data");
        Report = GetOfLib<FptrAction>("libfptr_report");
        ErrorCode = GetOfLib<FptrAction>("libfptr_error_code");
        Open = GetOfLib<FptrAction>("libfptr_open");
        Close = GetOfLib<FptrAction>("libfptr_close");
        OperatorLogin = GetOfLib<FptrAction>("libfptr_operator_login");
        CloseReceipt = GetOfLib<FptrAction>("libfptr_close_receipt");
        CancelReceipt = GetOfLib<FptrAction>("libfptr_cancel_receipt");
        Payment = GetOfLib<FptrAction>("libfptr_payment");
        ReceiptTotal = GetOfLib<FptrAction>("libfptr_receipt_total");
        OpenShift = GetOfLib<FptrAction>("libfptr_open_shift");
        GetParamBool = GetOfLib<FptrGetBoolean>("libfptr_get_param_bool");
        GetParamInt = GetOfLib<FptrGetInteger>("libfptr_get_param_int");
        GetParamByteArray = GetOfLib<FptrGetByteArray>("libfptr_get_param_bytearray");
        GetParamStr = GetOfLib<FptrGetByteArray>("libfptr_get_param_str");
        GetParamDate = GetOfLib<FptrGetDate>("libfptr_get_param_datetime");
        SetParamBool = GetOfLib<FptrSetBoolean>("libfptr_set_param_bool");
        SetParamInt = GetOfLib<FptrSetInteger>("libfptr_set_param_int");
        SetParamDouble = GetOfLib<FptrSetDouble>("libfptr_set_param_double");
        SetParamByteArray = GetOfLib<FptrSetByteArray>("libfptr_set_param_bytearray");
        SetParamStr = GetOfLib<FptrSetString>("libfptr_set_param_str");
        SetParamDate = GetOfLib<FptrSetDate>("libfptr_set_param_datetime");
        ShowProperties = GetOfLib<FptrShowProperties>("libfptr_show_properties");
        ErrorDescription = GetOfLib<FptrErrorDescription>("libfptr_error_description");
        GetSingleSetting = GetOfLib<FptrGetSetting>("libfptr_get_single_setting");
        IsOpened = GetOfLib<FptrGetFlag>("libfptr_is_opened");
    }

    public static void Load() => Instance = new();
    public static void Free()
    {
        foreach (var handle in Instance.handles)
            handle.Dispose();
        Instance.handles.Clear();
        Instance.Create = null!;
        Instance.Destroy = null!;
        Instance.Registration = null!;
        Instance.OpenReceipt = null!;
        Instance.UtilFormTLV = null!;
        Instance.FnQueryData = null!;
        Instance.QueryData = null!;
        Instance.Report = null!;
        Instance.ErrorCode = null!;
        Instance.Open = null!;
        Instance.Close = null!;
        Instance.OperatorLogin = null!;
        Instance.CloseReceipt = null!;
        Instance.CancelReceipt = null!;
        Instance.Payment = null!;
        Instance.ReceiptTotal = null!;
        Instance.OpenShift = null!;
        Instance.GetParamBool = null!;
        Instance.GetParamInt = null!;
        Instance.GetParamByteArray = null!;
        Instance.GetParamStr = null!;
        Instance.GetParamDate = null!;
        Instance.SetParamBool = null!;
        Instance.SetParamInt = null!;
        Instance.SetParamDouble = null!;
        Instance.SetParamByteArray = null!;
        Instance.SetParamStr = null!;
        Instance.SetParamDate = null!;
        Instance.ShowProperties = null!;
        Instance.ErrorDescription = null!;
        Instance.GetSingleSetting = null!;
        Instance.IsOpened = null!;
        NativeLibrary.Free(Instance.hLibrary);
        Instance = null!;
    }

    public static Fptr CreateHandle()
    {
        var instance = new Fptr();
        Instance.handles.Add(instance);
        return instance;
    }

    private T GetOfLib<T>(string nameMethod)
        where T : Delegate
    {
        var proc = NativeLibrary.GetExport(hLibrary, nameMethod);
        return Marshal.GetDelegateForFunctionPointer<T>(proc);
    }

    internal string GetParamString(nint handle, int paramId)
    {
        var bytes = new byte[DefaultSizeBytes];
        var len = GetParamStr(handle, paramId, bytes, bytes.Length);
        if (len > bytes.Length)
        {
            Array.Resize(ref bytes, len);
            GetParamStr(handle, paramId, bytes, bytes.Length);
        }
        return Encoding.GetString(bytes);
    }

    internal byte[] GetParamBytes(nint handle, int paramId)
    {
        var bytes = new byte[DefaultSizeBytes];
        var len = GetParamByteArray(handle, paramId, bytes, bytes.Length);
        if (len > bytes.Length)
        {
            Array.Resize(ref bytes, len);
            GetParamByteArray(handle, paramId, bytes, bytes.Length);
        }
        return bytes;
    }

    internal DateTime GetParamDateTime(nint handle, int paramId)
    {
        GetParamDate(handle, paramId, out var y, out var M, out var d, out var h, out var m, out var s);
        return new DateTime(y.ToInt32(), M.ToInt32(), d.ToInt32(), h.ToInt32(), m.ToInt32(), s.ToInt32(), (DateTimeKind)0);
    }

    internal void SetParamDateTime(nint handle, int paramId, DateTime value) =>
        SetParamDate(handle, paramId,
            value.Year, value.Month, value.Day,
            value.Hour, value.Minute, value.Second);

    internal void SetParamString(nint handle, int paramId, string value) =>
        SetParamStr(handle, paramId, Encoding.GetBytes(value));
}

#pragma warning disable S107
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int FptrHandleManagement(ref nint handle);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int FptrAction(nint handle);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate bool FptrGetBoolean(nint handle, int paramId);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void FptrSetBoolean(nint handle, int paramId, bool value);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate uint FptrGetInteger(nint handle, int paramId);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void FptrSetInteger(nint handle, int paramId, uint value);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate double FptrGetDouble(nint handle, int paramId);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void FptrSetDouble(nint handle, int paramId, double value);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void FptrSetString(nint handle, int paramId, byte[] value);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int FptrGetByteArray(nint handle, int paramId, byte[] value, int size);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void FptrSetByteArray(nint handle, int paramId, byte[] value, int size);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void FptrSetDate(nint handle, int paramId, int year, int month, int day, int hour, int minute, int second);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void FptrGetDate(nint handle, int paramId, out nint year, out nint month, out nint day, out nint hour, out nint minute, out nint second);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int FptrErrorDescription(nint handle, byte[] value, int size);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int FptrShowProperties(nint handle, int parentType, nint parent);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int FptrGetSetting(nint handle, byte[] key, byte[] value, int size);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate bool FptrGetFlag(nint handle);
#pragma warning restore S107