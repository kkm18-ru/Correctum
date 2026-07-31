using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace Correctum.Utils.Integrations
{
    internal sealed partial class FptrHandle : SafeHandle
    {
        private const string regeditKeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\ATOL\Drivers\10.0\KKT";
        private const int defaultStringSize = 128;
        private const int defaultBytesSize = 128;
        private static readonly Encoding encoding = Encoding.Unicode;
        private static readonly int sizeCharacter = encoding.GetByteCount(['\0']);

        private static readonly nint hLibrary;

        static FptrHandle()
        {
            var dllPath = (string)Registry.GetValue(regeditKeyName, "INSTALL_DIR", "")!;
            if (dllPath == "") throw new ArgumentNullException(null, "Не найден путь к драйверу ККТ Atol");
            dllPath = Path.Combine(dllPath, "bin", "fptr10.dll");
            hLibrary = NativeLibrary.Load(dllPath);

            createHandle = GetDelegateOfLib<HandleManagementDelegate>("libfptr_create");
            destroyHandle = GetDelegateOfLib<HandleManagementDelegate>("libfptr_destroy");

            registration = GetDelegateOfLib<SimpleDelegate>("libfptr_registration");
            openReceipt = GetDelegateOfLib<SimpleDelegate>("libfptr_open_receipt");
            utilFormTLV = GetDelegateOfLib<SimpleDelegate>("libfptr_util_form_tlv");
            fnQueryData = GetDelegateOfLib<SimpleDelegate>("libfptr_fn_query_data");
            queryData = GetDelegateOfLib<SimpleDelegate>("libfptr_query_data");
            report = GetDelegateOfLib<SimpleDelegate>("libfptr_report");
            errorCode = GetDelegateOfLib<SimpleDelegate>("libfptr_error_code");
            open = GetDelegateOfLib<SimpleDelegate>("libfptr_open");
            close = GetDelegateOfLib<SimpleDelegate>("libfptr_close");
            operatorLogin = GetDelegateOfLib<SimpleDelegate>("libfptr_operator_login");
            closeReceipt = GetDelegateOfLib<SimpleDelegate>("libfptr_close_receipt");
            cancelReceipt = GetDelegateOfLib<SimpleDelegate>("libfptr_cancel_receipt");
            payment = GetDelegateOfLib<SimpleDelegate>("libfptr_payment");
            receiptTotal = GetDelegateOfLib<SimpleDelegate>("libfptr_receipt_total");
            openShift = GetDelegateOfLib<SimpleDelegate>("libfptr_open_shift");

            getParamBool = GetDelegateOfLib<GetBoolDelegate>("libfptr_get_param_bool");
            getParamInt = GetDelegateOfLib<GetIntDelegate>("libfptr_get_param_int");
            //getParamDouble = GetDelegateOfLib<GetDoubleDelegate>("libfptr_get_param_double");
            getParamByteArray = GetDelegateOfLib<GetBytesDelegate>("libfptr_get_param_bytearray");
            getParamStr = GetDelegateOfLib<GetBytesDelegate>("libfptr_get_param_str");
            getParamDateTime = GetDelegateOfLib<GetDateDelegate>("libfptr_get_param_datetime");

            setParamBool = GetDelegateOfLib<SetBoolDelegate>("libfptr_set_param_bool");
            setParamInt = GetDelegateOfLib<SetIntDelegate>("libfptr_set_param_int");
            setParamDouble = GetDelegateOfLib<SetDoubleDelegate>("libfptr_set_param_double");
            setParamByteArray = GetDelegateOfLib<SetBytesDelegate>("libfptr_set_param_bytearray");
            setParamStr = GetDelegateOfLib<SetStrDelegate>("libfptr_set_param_str");
            setParamDateTime = GetDelegateOfLib<SetDateDelegate>("libfptr_set_param_datetime");

            showProperties = GetDelegateOfLib<ShowPropertiesDelegate>("libfptr_show_properties");

            errorDescription = GetDelegateOfLib<GetErrorDescriptionDelegate>("libfptr_error_description");
            
            getSingleSetting = GetDelegateOfLib<GetSettingDelegate>("libfptr_get_single_setting");
            
            isOpened = GetDelegateOfLib<ReturnBoolDelegate>("libfptr_is_opened");
        }

        public FptrHandle() : base(nint.Zero, true) => createHandle(out handle);

        public override bool IsInvalid => handle == nint.Zero;

        protected override bool ReleaseHandle() =>
            destroyHandle(out handle) == 0 && handle == nint.Zero;

        public static void FreeLibrary()
        {
            if (hLibrary != IntPtr.Zero)
                NativeLibrary.Free(hLibrary);
        }
    }
}
