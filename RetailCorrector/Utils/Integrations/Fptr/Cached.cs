namespace RetailCorrector.Utils.Integrations
{
    partial class FptrHandle
    {
        private readonly static HandleManagementDelegate createHandle;          // libfptr_create
        private readonly static HandleManagementDelegate destroyHandle;         // libfptr_destroy

        private readonly static SimpleDelegate registration;                    // libfptr_registration
        private readonly static SimpleDelegate openReceipt;                     // libfptr_open_receipt
        private readonly static SimpleDelegate utilFormTLV;                     // libfptr_util_form_tlv
        private readonly static SimpleDelegate fnQueryData;                     // libfptr_fn_query_data
        private readonly static SimpleDelegate queryData;                       // libfptr_query_data
        private readonly static SimpleDelegate report;                          // libfptr_report
        private readonly static SimpleDelegate errorCode;                       // libfptr_error_code
        private readonly static SimpleDelegate open;                            // libfptr_open
        private readonly static SimpleDelegate close;                           // libfptr_close
        private readonly static SimpleDelegate operatorLogin;                   // libfptr_operator_login
        private readonly static SimpleDelegate deviceReboot;                    // libfptr_device_reboot
        private readonly static SimpleDelegate closeReceipt;                    // libfptr_close_receipt
        private readonly static SimpleDelegate cancelReceipt;                   // libfptr_cancel_receipt
        private readonly static SimpleDelegate payment;                         // libfptr_payment
        private readonly static SimpleDelegate receiptTotal;                    // libfptr_receipt_total
        private readonly static SimpleDelegate openShift;                       // libfptr_open_shift

        private readonly static GetBoolDelegate getParamBool;                   // libfptr_get_param_bool
        private readonly static GetIntDelegate getParamInt;                     // libfptr_get_param_int
        private readonly static GetDoubleDelegate getParamDouble;               // libfptr_get_param_double
        private readonly static GetBytesDelegate getParamByteArray;             // libfptr_get_param_bytearray
        private readonly static GetBytesDelegate getParamStr;                   // libfptr_get_param_str
        private readonly static GetDateDelegate getParamDateTime;               // libfptr_get_param_datetime

        private readonly static SetBoolDelegate setParamBool;                   // libfptr_set_param_bool
        private readonly static SetIntDelegate setParamInt;                     // libfptr_set_param_int
        private readonly static SetDoubleDelegate setParamDouble;               // libfptr_set_param_double
        private readonly static SetBytesDelegate setParamByteArray;             // libfptr_set_param_bytearray
        private readonly static SetStrDelegate setParamStr;                     // libfptr_set_param_str
        private readonly static SetDateDelegate setParamDateTime;               // libfptr_set_param_datetime

        private readonly static ShowPropertiesDelegate showProperties;          // libfptr_show_properties

        private readonly static GetErrorDescriptionDelegate errorDescription;   // libfptr_error_description
    }
}
