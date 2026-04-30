namespace RetailCorrector.Utils.Integrations
{
    partial class FptrHandle
    {
        private static readonly HandleManagementDelegate createHandle;          // libfptr_create
        private static readonly HandleManagementDelegate destroyHandle;         // libfptr_destroy

        private static readonly SimpleDelegate registration;                    // libfptr_registration
        private static readonly SimpleDelegate openReceipt;                     // libfptr_open_receipt
        private static readonly SimpleDelegate utilFormTLV;                     // libfptr_util_form_tlv
        private static readonly SimpleDelegate fnQueryData;                     // libfptr_fn_query_data
        private static readonly SimpleDelegate queryData;                       // libfptr_query_data
        private static readonly SimpleDelegate report;                          // libfptr_report
        private static readonly SimpleDelegate errorCode;                       // libfptr_error_code
        private static readonly SimpleDelegate open;                            // libfptr_open
        private static readonly SimpleDelegate close;                           // libfptr_close
        private static readonly SimpleDelegate operatorLogin;                   // libfptr_operator_login
        private static readonly SimpleDelegate closeReceipt;                    // libfptr_close_receipt
        private static readonly SimpleDelegate cancelReceipt;                   // libfptr_cancel_receipt
        private static readonly SimpleDelegate payment;                         // libfptr_payment
        private static readonly SimpleDelegate receiptTotal;                    // libfptr_receipt_total
        private static readonly SimpleDelegate openShift;                       // libfptr_open_shift

        private static readonly GetBoolDelegate getParamBool;                   // libfptr_get_param_bool
        private static readonly GetIntDelegate getParamInt;                     // libfptr_get_param_int
        //private static readonly GetDoubleDelegate getParamDouble;             // libfptr_get_param_double
        private static readonly GetBytesDelegate getParamByteArray;             // libfptr_get_param_bytearray
        private static readonly GetBytesDelegate getParamStr;                   // libfptr_get_param_str
        private static readonly GetDateDelegate getParamDateTime;               // libfptr_get_param_datetime

        private static readonly SetBoolDelegate setParamBool;                   // libfptr_set_param_bool
        private static readonly SetIntDelegate setParamInt;                     // libfptr_set_param_int
        private static readonly SetDoubleDelegate setParamDouble;               // libfptr_set_param_double
        private static readonly SetBytesDelegate setParamByteArray;             // libfptr_set_param_bytearray
        private static readonly SetStrDelegate setParamStr;                     // libfptr_set_param_str
        private static readonly SetDateDelegate setParamDateTime;               // libfptr_set_param_datetime

        private static readonly ShowPropertiesDelegate showProperties;          // libfptr_show_properties

        private static readonly GetErrorDescriptionDelegate errorDescription;   // libfptr_error_description
        
        private static readonly GetSettingDelegate getSingleSetting;            // libfptr_get_single_setting
        
        private static readonly ReturnBoolDelegate isOpened;                    // libfptr_is_opened
    }
}
