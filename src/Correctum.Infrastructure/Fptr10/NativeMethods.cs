using System.Runtime.InteropServices;

namespace Correctum.Fptr10;

public partial class Fptr
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpPathName);

    [DllImport("libdl.so.2", EntryPoint = "dlopen")]
    private static extern IntPtr dlopen_linux(string lpPathName, int flags);

    [DllImport("libdl", EntryPoint = "dlopen")]
    private static extern IntPtr dlopen_osx(string lpPathName, int flags);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr libfptr_get_version_string();

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_create(out IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_create_with_id(out IntPtr handle, byte[] id);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_destroy(out IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_log_write_ex(IntPtr handle, byte[] tag, int level, byte[] message);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_change_label(IntPtr handle, byte[] label);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_show_properties(IntPtr handle, int parentType, IntPtr parent);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool libfptr_is_opened(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_error_code(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_error_description(IntPtr handle, byte[] value, int size);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_error_recommendation(IntPtr handle, byte[] value, int size);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_reset_error(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_set_settings(IntPtr handle, byte[] settings);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_get_settings(IntPtr handle, byte[] value, int size);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_single_setting(IntPtr handle, byte[] key, byte[] setting);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_get_single_setting(IntPtr handle, byte[] key, byte[] value, int size);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_param_int(IntPtr handle, int paramID, uint value);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_param_bool(IntPtr handle, int paramID, bool value);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_param_double(IntPtr handle, int paramID, double value);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_param_bytearray(IntPtr handle, int paramID, byte[] value, int size);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_param_datetime(IntPtr handle, int paramID, int year, int month, int day, int hour, int minute, int second);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_param_str(IntPtr handle, int paramID, byte[] value);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_non_printable_param_int(IntPtr handle, int paramID, uint value);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_non_printable_param_bool(IntPtr handle, int paramID, bool value);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_non_printable_param_double(IntPtr handle, int paramID, double value);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_non_printable_param_bytearray(IntPtr handle, int paramID, byte[] value, int size);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_non_printable_param_datetime(IntPtr handle, int paramID, int year, int month, int day, int hour, int minute, int second);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_non_printable_param_str(IntPtr handle, int paramID, byte[] value);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_user_param_int(IntPtr handle, int paramID, uint value);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_user_param_bool(IntPtr handle, int paramID, bool value);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_user_param_double(IntPtr handle, int paramID, double value);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_user_param_bytearray(IntPtr handle, int paramID, byte[] value, int size);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_user_param_datetime(IntPtr handle, int paramID, int year, int month, int day, int hour, int minute, int second);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_set_user_param_str(IntPtr handle, int paramID, byte[] value);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint libfptr_get_param_int(IntPtr handle, int paramID);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool libfptr_get_param_bool(IntPtr handle, int paramID);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern double libfptr_get_param_double(IntPtr handle, int paramID);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_get_param_bytearray(IntPtr handle, int paramID, byte[] value, int size);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern void libfptr_get_param_datetime(IntPtr handle, int paramID, out IntPtr year, out IntPtr month, out IntPtr day, out IntPtr hour, out IntPtr minute, out IntPtr second);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_get_param_str(IntPtr handle, int paramID, byte[] value, int size);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool libfptr_is_param_available(IntPtr handle, int paramID);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_apply_single_settings(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_open(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_close(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_reset_params(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_run_command(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_beep(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_open_drawer(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_cut(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_device_poweroff(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_device_reboot(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_open_shift(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_reset_summary(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_init_device(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_query_data(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_cash_income(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_cash_outcome(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_open_receipt(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_cancel_receipt(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_close_receipt(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_check_document_closed(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_receipt_total(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_receipt_tax(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_registration(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_payment(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_report(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_print_text(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_print_cliche(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_begin_nonfiscal_document(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_end_nonfiscal_document(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_print_barcode(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_print_picture(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_print_picture_by_number(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_upload_picture_from_file(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_clear_pictures(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_write_device_setting_raw(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_read_device_setting_raw(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_commit_settings(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_init_settings(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_reset_settings(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_write_date_time(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_write_license(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_fn_operation(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_fn_query_data(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_fn_write_attributes(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_external_device_power_on(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_external_device_power_off(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_external_device_write_data(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_external_device_read_data(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_operator_login(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_process_json(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_read_device_setting(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_write_device_setting(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_begin_read_records(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_read_next_record(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_end_read_records(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_user_memory_operation(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_continue_print(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_init_mgm(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_util_form_tlv(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_util_form_nomenclature(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_util_mapping(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_read_model_flags(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_line_feed(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_flash_firmware(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_soft_lock_init(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_soft_lock_query_session_code(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_soft_lock_validate(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_util_calc_tax(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_download_picture(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_bluetooth_remove_paired_devices(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_util_tag_info(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_util_container_versions(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_activate_licenses(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_remove_licenses(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_enter_keys(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_validate_keys(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_enter_serial_number(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_get_serial_number_request(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_upload_pixel_buffer(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_download_pixel_buffer(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_print_pixel_buffer(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_util_convert_tag_value(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_parse_marking_code(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_call_script(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_set_header_lines(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_set_footer_lines(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_upload_picture_cliche(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_upload_picture_memory(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_upload_pixel_buffer_cliche(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_upload_pixel_buffer_memory(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_exec_driver_script(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_upload_driver_script(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_exec_driver_script_by_id(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_write_universal_counters_settings(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_read_universal_counters_settings(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_query_universal_counters_state(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_reset_universal_counters(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_cache_universal_counters(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_read_universal_counter_sum(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_read_universal_counter_quantity(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_clear_universal_counters_cache(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_disable_ofd_channel(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_enable_ofd_channel(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_validate_json(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_reflection_call(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_get_remote_server_info(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_begin_marking_code_validation(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_cancel_marking_code_validation(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_get_marking_code_validation_status(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_accept_marking_code(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_decline_marking_code(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_update_fnm_keys(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_write_sales_notice(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_check_marking_code_validations_ready(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_clear_marking_code_validation_result(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_ping_marking_server(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_get_marking_server_status(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_is_driver_locked(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_get_last_document_journal(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_find_document_in_journal(IntPtr handle);

    [DllImport("fptr10", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_run_fn_command(IntPtr handle);

}
