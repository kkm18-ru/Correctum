using System.Runtime.InteropServices;
using System.Text;

namespace RetailCorrector.Utils
{
    internal static partial class FptrNative
    {
        [LibraryImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr LoadLibrary(string lpPathName);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool FreeLibrary(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void libfptr_set_param_int(IntPtr handle, int paramID, uint value);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_query_data(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial uint libfptr_get_param_int(IntPtr handle, int paramID);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_device_reboot(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_fn_query_data(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_operator_login(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void libfptr_set_param_bool(IntPtr handle, int paramID, [MarshalAs(UnmanagedType.Bool)] bool value);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void libfptr_set_param_double(IntPtr handle, int paramID, double value);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void libfptr_set_param_bytearray(IntPtr handle, int paramID, byte[] value, int size);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void libfptr_set_param_datetime(IntPtr handle, int paramID, int year, int month, int day, int hour, int minute, int second);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void libfptr_set_param_str(IntPtr handle, int paramID, [MarshalAs(UnmanagedType.LPWStr)] string value);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_registration(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_payment(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_open_receipt(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_cancel_receipt(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_close_receipt(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_util_form_tlv(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool libfptr_get_param_bool(IntPtr handle, int paramID);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial double libfptr_get_param_double(IntPtr handle, int paramID);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_get_param_bytearray(IntPtr handle, int paramID, byte[] value, int size);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void libfptr_get_param_datetime(IntPtr handle, int paramID, out IntPtr year, out IntPtr month, out IntPtr day, out IntPtr hour, out IntPtr minute, out IntPtr second);

        [DllImport("fptr10.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int libfptr_get_param_str(IntPtr handle, int paramID, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder value, int size);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_receipt_total(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_open(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_close(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_error_code(IntPtr handle);

        [DllImport("fptr10.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int libfptr_error_description(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder value, int size);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_create(out IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_destroy(out IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_report(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_open_shift(IntPtr handle);

        [LibraryImport("fptr10.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int libfptr_show_properties(IntPtr handle, int parentType, IntPtr parent);
    }
}
