using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Correctum.Fptr10;

public partial class Fptr
{
    private IntPtr m_handle = IntPtr.Zero;

    
    private static void raiseNotFoundError(string path, Exception reason)
    {
        if (path.Length == 0)
        {
            throw new FileNotFoundException("Driver library not found in search folders", reason);
        }

        try
        {
            if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
            {
                throw new FileNotFoundException("Driver library not found in \"" + path + "\"", reason);
            }
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException("Can`t load driver library \"" + path + "\"", reason);
        }
        catch (DirectoryNotFoundException)
        {
            throw new FileNotFoundException("Driver library not found in \"" + path + "\"", reason);
        }

        throw new FileNotFoundException("Can`t load driver library \"" + path + "\"", reason);
    }

    private static bool doLoadDriver(string path)
    {
        try
        {
            if (OSDetector.Type == OSDetector.OSType.Windows)
            {
                if (path.Length == 0)
                {
                    path = "fptr10.dll";
                }
                else if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    path = Path.Combine(path, "fptr10.dll");
                }

                IntPtr intPtr = LoadLibrary(path);
                if (intPtr == IntPtr.Zero)
                {
                    LoadLibrary(Path.Combine(Path.GetDirectoryName(path), "msvcp140.dll"));
                    intPtr = LoadLibrary(path);
                }

                return intPtr != IntPtr.Zero;
            }

            if (OSDetector.Type == OSDetector.OSType.Linux)
            {
                if (path.Length == 0)
                {
                    path = "libfptr10.so";
                }
                else if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    path = Path.Combine(path, "libfptr10.so");
                }

                return dlopen_linux(path, 2) != IntPtr.Zero;
            }

            if (OSDetector.Type == OSDetector.OSType.OSX)
            {
                if (path.Length == 0)
                {
                    path = "libfptr10.dylib";
                }
                else if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    path = Path.Combine(path, "libfptr10.dylib");
                }

                return dlopen_linux(path, 2) != IntPtr.Zero;
            }

            throw new PlatformNotSupportedException();
        }
        catch (FileNotFoundException reason)
        {
            raiseNotFoundError(path, reason);
            return false;
        }
    }

    private static void loadDriver(string path)
    {
        try
        {
            libfptr_get_version_string();
            return;
        }
        catch (Exception)
        {
        }

        path = path ?? "";
        if (path.Length == 0)
        {
            if (doLoadDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
            {
                return;
            }

            if (OSDetector.Type == OSDetector.OSType.Windows)
            {
                path = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\ATOL\\Drivers\\10.0\\KKT", "INSTALL_DIR", null);
                if (path == null)
                {
                    throw new FileNotFoundException("Driver not installed");
                }

                path = Path.Combine(path, "bin");
            }
        }

        if (!doLoadDriver(path))
        {
            raiseNotFoundError(path, null);
        }
    }

    public Fptr()
    {
        loadDriver(null);
        switch (libfptr_create(out m_handle))
        {
            case -2:
                throw new ArgumentException("Invalid [id] format");
            default:
                throw new Exception("Can`t create driver handle");
            case 0:
                break;
        }
    }

    public Fptr(string libraryPath)
    {
        loadDriver(libraryPath);
        switch (libfptr_create(out m_handle))
        {
            case -2:
                throw new ArgumentException("Invalid [id] format");
            default:
                throw new Exception("Can`t create driver handle");
            case 0:
                break;
        }
    }

    public Fptr(string id, string libraryPath)
    {
        loadDriver(libraryPath);
        switch (libfptr_create_with_id(out m_handle, OSDetector.Encoding.GetBytes(id)))
        {
            case -2:
                throw new ArgumentException("Invalid [id] format");
            default:
                throw new Exception("Can`t create driver handle");
            case 0:
                break;
        }
    }

    ~Fptr()
    {
        if (m_handle != IntPtr.Zero)
        {
            libfptr_destroy(out m_handle);
        }
    }

    public void destroy()
    {
        if (m_handle != IntPtr.Zero)
        {
            libfptr_destroy(out m_handle);
        }
    }

    public string version()
    {
        return Marshal.PtrToStringAnsi(libfptr_get_version_string());
    }

    public string wrapperVersion()
    {
        return "10.10.8.0";
    }

    public int logWrite(string tag, int level, string message)
    {
        return libfptr_log_write_ex(m_handle, OSDetector.Encoding.GetBytes(tag), level, OSDetector.Encoding.GetBytes(message));
    }

    public int changeLabel(string label)
    {
        return libfptr_change_label(m_handle, OSDetector.Encoding.GetBytes(label));
    }

    public int showProperties(int parentType, IntPtr parent)
    {
        return libfptr_show_properties(m_handle, parentType, parent);
    }

    public bool isOpened()
    {
        return libfptr_is_opened(m_handle);
    }

    public int errorCode()
    {
        return libfptr_error_code(m_handle);
    }

    public string errorDescription()
    {
        int num = 128;
        byte[] array = new byte[OSDetector.SymbolSize * num];
        int num2 = libfptr_error_description(m_handle, array, num);
        if (num2 > num)
        {
            num = num2;
            array = new byte[OSDetector.SymbolSize * num2];
            libfptr_error_description(m_handle, array, num);
        }

        return OSDetector.Encoding.GetString(array, 0, num2 * OSDetector.SymbolSize).TrimEnd(new char[1]);
    }

    public string errorRecommendation()
    {
        int num = 128;
        byte[] array = new byte[OSDetector.SymbolSize * num];
        int num2 = libfptr_error_recommendation(m_handle, array, num);
        if (num2 > num)
        {
            num = num2;
            array = new byte[OSDetector.SymbolSize * num2];
            libfptr_error_recommendation(m_handle, array, num);
        }

        return OSDetector.Encoding.GetString(array, 0, num2 * OSDetector.SymbolSize).TrimEnd(new char[1]);
    }

    public void resetError()
    {
        libfptr_reset_error(m_handle);
    }

    public int setSettings(string settings)
    {
        return libfptr_set_settings(m_handle, OSDetector.Encoding.GetBytes(settings));
    }

    public string getSettings()
    {
        int num = 128;
        byte[] array = new byte[OSDetector.SymbolSize * num];
        int num2 = libfptr_get_settings(m_handle, array, num);
        if (num2 > num)
        {
            num = num2;
            array = new byte[OSDetector.SymbolSize * num2];
            libfptr_get_settings(m_handle, array, num);
        }

        return OSDetector.Encoding.GetString(array, 0, num2 * OSDetector.SymbolSize).TrimEnd(new char[1]);
    }

    public void setSingleSetting(string key, string setting)
    {
        libfptr_set_single_setting(m_handle, OSDetector.Encoding.GetBytes(key), OSDetector.Encoding.GetBytes(setting));
    }

    public string getSingleSetting(string key)
    {
        int num = 128;
        byte[] array = new byte[OSDetector.SymbolSize * num];
        int num2 = libfptr_get_single_setting(m_handle, OSDetector.Encoding.GetBytes(key), array, num);
        if (num2 > num)
        {
            num = num2;
            array = new byte[OSDetector.SymbolSize * num2];
            libfptr_get_single_setting(m_handle, OSDetector.Encoding.GetBytes(key), array, num);
        }

        return OSDetector.Encoding.GetString(array, 0, num2 * OSDetector.SymbolSize).TrimEnd(new char[1]);
    }

    public void setParam(int paramID, uint value)
    {
        libfptr_set_param_int(m_handle, paramID, value);
    }

    public void setParam(int paramID, int value)
    {
        if (value < 0)
        {
            libfptr_set_param_double(m_handle, paramID, value);
        }
        else
        {
            libfptr_set_param_int(m_handle, paramID, (uint)value);
        }
    }

    public void setParam(int paramID, bool value)
    {
        libfptr_set_param_bool(m_handle, paramID, value);
    }

    public void setParam(int paramID, double value)
    {
        libfptr_set_param_double(m_handle, paramID, value);
    }

    public void setParam(int paramID, byte[] value)
    {
        libfptr_set_param_bytearray(m_handle, paramID, value, value.Length);
    }

    public void setParam(int paramID, DateTime value)
    {
        libfptr_set_param_datetime(m_handle, paramID, value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
    }

    public void setParam(int paramID, string value)
    {
        libfptr_set_param_str(m_handle, paramID, OSDetector.Encoding.GetBytes(value));
    }

    public void setNonPrintableParam(int paramID, uint value)
    {
        libfptr_set_non_printable_param_int(m_handle, paramID, value);
    }

    public void setNonPrintableParam(int paramID, int value)
    {
        if (value < 0)
        {
            libfptr_set_non_printable_param_double(m_handle, paramID, value);
        }
        else
        {
            libfptr_set_non_printable_param_int(m_handle, paramID, (uint)value);
        }
    }

    public void setNonPrintableParam(int paramID, bool value)
    {
        libfptr_set_non_printable_param_bool(m_handle, paramID, value);
    }

    public void setNonPrintableParam(int paramID, double value)
    {
        libfptr_set_non_printable_param_double(m_handle, paramID, value);
    }

    public void setNonPrintableParam(int paramID, byte[] value)
    {
        libfptr_set_non_printable_param_bytearray(m_handle, paramID, value, value.Length);
    }

    public void setNonPrintableParam(int paramID, DateTime value)
    {
        libfptr_set_non_printable_param_datetime(m_handle, paramID, value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
    }

    public void setNonPrintableParam(int paramID, string value)
    {
        libfptr_set_non_printable_param_str(m_handle, paramID, OSDetector.Encoding.GetBytes(value));
    }

    public void setUserParam(int paramID, uint value)
    {
        libfptr_set_user_param_int(m_handle, paramID, value);
    }

    public void setUserParam(int paramID, int value)
    {
        if (value < 0)
        {
            libfptr_set_user_param_double(m_handle, paramID, value);
        }
        else
        {
            libfptr_set_user_param_int(m_handle, paramID, (uint)value);
        }
    }

    public void setUserParam(int paramID, bool value)
    {
        libfptr_set_user_param_bool(m_handle, paramID, value);
    }

    public void setUserParam(int paramID, double value)
    {
        libfptr_set_user_param_double(m_handle, paramID, value);
    }

    public void setUserParam(int paramID, byte[] value)
    {
        libfptr_set_user_param_bytearray(m_handle, paramID, value, value.Length);
    }

    public void setUserParam(int paramID, DateTime value)
    {
        libfptr_set_user_param_datetime(m_handle, paramID, value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
    }

    public void setUserParam(int paramID, string value)
    {
        libfptr_set_user_param_str(m_handle, paramID, OSDetector.Encoding.GetBytes(value));
    }

    public uint getParamInt(int paramID)
    {
        return libfptr_get_param_int(m_handle, paramID);
    }

    public bool getParamBool(int paramID)
    {
        return libfptr_get_param_bool(m_handle, paramID);
    }

    public double getParamDouble(int paramID)
    {
        return libfptr_get_param_double(m_handle, paramID);
    }

    public byte[] getParamByteArray(int paramID)
    {
        byte[] array = new byte[128];
        int num = libfptr_get_param_bytearray(m_handle, paramID, array, array.Length);
        if (num > array.Length)
        {
            Array.Resize(ref array, num);
            libfptr_get_param_bytearray(m_handle, paramID, array, array.Length);
        }
        else
        {
            Array.Resize(ref array, num);
        }

        return array;
    }

    public DateTime getParamDateTime(int paramID)
    {
        libfptr_get_param_datetime(m_handle, paramID, out var year, out var month, out var day, out var hour, out var minute, out var second);
        return new DateTime(year.ToInt32(), month.ToInt32(), day.ToInt32(), hour.ToInt32(), minute.ToInt32(), second.ToInt32());
    }

    public string getParamString(int paramID)
    {
        int num = 128;
        byte[] array = new byte[OSDetector.SymbolSize * num];
        int num2 = libfptr_get_param_str(m_handle, paramID, array, num);
        if (num2 > num)
        {
            num = num2;
            array = new byte[OSDetector.SymbolSize * num2];
            libfptr_get_param_str(m_handle, paramID, array, num);
        }

        return OSDetector.Encoding.GetString(array, 0, num2 * OSDetector.SymbolSize).TrimEnd(new char[1]);
    }

    public bool isParamAvailable(int paramID)
    {
        return libfptr_is_param_available(m_handle, paramID);
    }

    public int applySingleSettings()
    {
        return libfptr_apply_single_settings(m_handle);
    }

    public int open()
    {
        return libfptr_open(m_handle);
    }

    public int close()
    {
        return libfptr_close(m_handle);
    }

    public int resetParams()
    {
        return libfptr_reset_params(m_handle);
    }

    public int runCommand()
    {
        return libfptr_run_command(m_handle);
    }

    public int beep()
    {
        return libfptr_beep(m_handle);
    }

    public int openDrawer()
    {
        return libfptr_open_drawer(m_handle);
    }

    public int cut()
    {
        return libfptr_cut(m_handle);
    }

    public int devicePoweroff()
    {
        return libfptr_device_poweroff(m_handle);
    }

    public int deviceReboot()
    {
        return libfptr_device_reboot(m_handle);
    }

    public int openShift()
    {
        return libfptr_open_shift(m_handle);
    }

    public int resetSummary()
    {
        return libfptr_reset_summary(m_handle);
    }

    public int initDevice()
    {
        return libfptr_init_device(m_handle);
    }

    public int queryData()
    {
        return libfptr_query_data(m_handle);
    }

    public int cashIncome()
    {
        return libfptr_cash_income(m_handle);
    }

    public int cashOutcome()
    {
        return libfptr_cash_outcome(m_handle);
    }

    public int openReceipt()
    {
        return libfptr_open_receipt(m_handle);
    }

    public int cancelReceipt()
    {
        return libfptr_cancel_receipt(m_handle);
    }

    public int closeReceipt()
    {
        return libfptr_close_receipt(m_handle);
    }

    public int checkDocumentClosed()
    {
        return libfptr_check_document_closed(m_handle);
    }

    public int receiptTotal()
    {
        return libfptr_receipt_total(m_handle);
    }

    public int receiptTax()
    {
        return libfptr_receipt_tax(m_handle);
    }

    public int registration()
    {
        return libfptr_registration(m_handle);
    }

    public int payment()
    {
        return libfptr_payment(m_handle);
    }

    public int report()
    {
        return libfptr_report(m_handle);
    }

    public int printText()
    {
        return libfptr_print_text(m_handle);
    }

    public int printCliche()
    {
        return libfptr_print_cliche(m_handle);
    }

    public int beginNonfiscalDocument()
    {
        return libfptr_begin_nonfiscal_document(m_handle);
    }

    public int endNonfiscalDocument()
    {
        return libfptr_end_nonfiscal_document(m_handle);
    }

    public int printBarcode()
    {
        return libfptr_print_barcode(m_handle);
    }

    public int printPicture()
    {
        return libfptr_print_picture(m_handle);
    }

    public int printPictureByNumber()
    {
        return libfptr_print_picture_by_number(m_handle);
    }

    public int uploadPictureFromFile()
    {
        return libfptr_upload_picture_from_file(m_handle);
    }

    public int clearPictures()
    {
        return libfptr_clear_pictures(m_handle);
    }

    public int writeDeviceSettingRaw()
    {
        return libfptr_write_device_setting_raw(m_handle);
    }

    public int readDeviceSettingRaw()
    {
        return libfptr_read_device_setting_raw(m_handle);
    }

    public int commitSettings()
    {
        return libfptr_commit_settings(m_handle);
    }

    public int initSettings()
    {
        return libfptr_init_settings(m_handle);
    }

    public int resetSettings()
    {
        return libfptr_reset_settings(m_handle);
    }

    public int writeDateTime()
    {
        return libfptr_write_date_time(m_handle);
    }

    public int writeLicense()
    {
        return libfptr_write_license(m_handle);
    }

    public int fnOperation()
    {
        return libfptr_fn_operation(m_handle);
    }

    public int fnQueryData()
    {
        return libfptr_fn_query_data(m_handle);
    }

    public int fnWriteAttributes()
    {
        return libfptr_fn_write_attributes(m_handle);
    }

    public int externalDevicePowerOn()
    {
        return libfptr_external_device_power_on(m_handle);
    }

    public int externalDevicePowerOff()
    {
        return libfptr_external_device_power_off(m_handle);
    }

    public int externalDeviceWriteData()
    {
        return libfptr_external_device_write_data(m_handle);
    }

    public int externalDeviceReadData()
    {
        return libfptr_external_device_read_data(m_handle);
    }

    public int operatorLogin()
    {
        return libfptr_operator_login(m_handle);
    }

    public int processJson()
    {
        return libfptr_process_json(m_handle);
    }

    public int readDeviceSetting()
    {
        return libfptr_read_device_setting(m_handle);
    }

    public int writeDeviceSetting()
    {
        return libfptr_write_device_setting(m_handle);
    }

    public int beginReadRecords()
    {
        return libfptr_begin_read_records(m_handle);
    }

    public int readNextRecord()
    {
        return libfptr_read_next_record(m_handle);
    }

    public int endReadRecords()
    {
        return libfptr_end_read_records(m_handle);
    }

    public int userMemoryOperation()
    {
        return libfptr_user_memory_operation(m_handle);
    }

    public int continuePrint()
    {
        return libfptr_continue_print(m_handle);
    }

    public int initMgm()
    {
        return libfptr_init_mgm(m_handle);
    }

    public int utilFormTlv()
    {
        return libfptr_util_form_tlv(m_handle);
    }

    public int utilFormNomenclature()
    {
        return libfptr_util_form_nomenclature(m_handle);
    }

    public int utilMapping()
    {
        return libfptr_util_mapping(m_handle);
    }

    public int readModelFlags()
    {
        return libfptr_read_model_flags(m_handle);
    }

    public int lineFeed()
    {
        return libfptr_line_feed(m_handle);
    }

    public int flashFirmware()
    {
        return libfptr_flash_firmware(m_handle);
    }

    public int softLockInit()
    {
        return libfptr_soft_lock_init(m_handle);
    }

    public int softLockQuerySessionCode()
    {
        return libfptr_soft_lock_query_session_code(m_handle);
    }

    public int softLockValidate()
    {
        return libfptr_soft_lock_validate(m_handle);
    }

    public int utilCalcTax()
    {
        return libfptr_util_calc_tax(m_handle);
    }

    public int downloadPicture()
    {
        return libfptr_download_picture(m_handle);
    }

    public int bluetoothRemovePairedDevices()
    {
        return libfptr_bluetooth_remove_paired_devices(m_handle);
    }

    public int utilTagInfo()
    {
        return libfptr_util_tag_info(m_handle);
    }

    public int utilContainerVersions()
    {
        return libfptr_util_container_versions(m_handle);
    }

    public int activateLicenses()
    {
        return libfptr_activate_licenses(m_handle);
    }

    public int removeLicenses()
    {
        return libfptr_remove_licenses(m_handle);
    }

    public int enterKeys()
    {
        return libfptr_enter_keys(m_handle);
    }

    public int validateKeys()
    {
        return libfptr_validate_keys(m_handle);
    }

    public int enterSerialNumber()
    {
        return libfptr_enter_serial_number(m_handle);
    }

    public int getSerialNumberRequest()
    {
        return libfptr_get_serial_number_request(m_handle);
    }

    public int uploadPixelBuffer()
    {
        return libfptr_upload_pixel_buffer(m_handle);
    }

    public int downloadPixelBuffer()
    {
        return libfptr_download_pixel_buffer(m_handle);
    }

    public int printPixelBuffer()
    {
        return libfptr_print_pixel_buffer(m_handle);
    }

    public int utilConvertTagValue()
    {
        return libfptr_util_convert_tag_value(m_handle);
    }

    public int parseMarkingCode()
    {
        return libfptr_parse_marking_code(m_handle);
    }

    public int callScript()
    {
        return libfptr_call_script(m_handle);
    }

    public int setHeaderLines()
    {
        return libfptr_set_header_lines(m_handle);
    }

    public int setFooterLines()
    {
        return libfptr_set_footer_lines(m_handle);
    }

    public int uploadPictureCliche()
    {
        return libfptr_upload_picture_cliche(m_handle);
    }

    public int uploadPictureMemory()
    {
        return libfptr_upload_picture_memory(m_handle);
    }

    public int uploadPixelBufferCliche()
    {
        return libfptr_upload_pixel_buffer_cliche(m_handle);
    }

    public int uploadPixelBufferMemory()
    {
        return libfptr_upload_pixel_buffer_memory(m_handle);
    }

    public int execDriverScript()
    {
        return libfptr_exec_driver_script(m_handle);
    }

    public int uploadDriverScript()
    {
        return libfptr_upload_driver_script(m_handle);
    }

    public int execDriverScriptById()
    {
        return libfptr_exec_driver_script_by_id(m_handle);
    }

    public int writeUniversalCountersSettings()
    {
        return libfptr_write_universal_counters_settings(m_handle);
    }

    public int readUniversalCountersSettings()
    {
        return libfptr_read_universal_counters_settings(m_handle);
    }

    public int queryUniversalCountersState()
    {
        return libfptr_query_universal_counters_state(m_handle);
    }

    public int resetUniversalCounters()
    {
        return libfptr_reset_universal_counters(m_handle);
    }

    public int cacheUniversalCounters()
    {
        return libfptr_cache_universal_counters(m_handle);
    }

    public int readUniversalCounterSum()
    {
        return libfptr_read_universal_counter_sum(m_handle);
    }

    public int readUniversalCounterQuantity()
    {
        return libfptr_read_universal_counter_quantity(m_handle);
    }

    public int clearUniversalCountersCache()
    {
        return libfptr_clear_universal_counters_cache(m_handle);
    }

    public int disableOfdChannel()
    {
        return libfptr_disable_ofd_channel(m_handle);
    }

    public int enableOfdChannel()
    {
        return libfptr_enable_ofd_channel(m_handle);
    }

    public int validateJson()
    {
        return libfptr_validate_json(m_handle);
    }

    public int reflectionCall()
    {
        return libfptr_reflection_call(m_handle);
    }

    public int getRemoteServerInfo()
    {
        return libfptr_get_remote_server_info(m_handle);
    }

    public int beginMarkingCodeValidation()
    {
        return libfptr_begin_marking_code_validation(m_handle);
    }

    public int cancelMarkingCodeValidation()
    {
        return libfptr_cancel_marking_code_validation(m_handle);
    }

    public int getMarkingCodeValidationStatus()
    {
        return libfptr_get_marking_code_validation_status(m_handle);
    }

    public int acceptMarkingCode()
    {
        return libfptr_accept_marking_code(m_handle);
    }

    public int declineMarkingCode()
    {
        return libfptr_decline_marking_code(m_handle);
    }

    public int updateFnmKeys()
    {
        return libfptr_update_fnm_keys(m_handle);
    }

    public int writeSalesNotice()
    {
        return libfptr_write_sales_notice(m_handle);
    }

    public int checkMarkingCodeValidationsReady()
    {
        return libfptr_check_marking_code_validations_ready(m_handle);
    }

    public int clearMarkingCodeValidationResult()
    {
        return libfptr_clear_marking_code_validation_result(m_handle);
    }

    public int pingMarkingServer()
    {
        return libfptr_ping_marking_server(m_handle);
    }

    public int getMarkingServerStatus()
    {
        return libfptr_get_marking_server_status(m_handle);
    }

    public int isDriverLocked()
    {
        return libfptr_is_driver_locked(m_handle);
    }

    public int getLastDocumentJournal()
    {
        return libfptr_get_last_document_journal(m_handle);
    }

    public int findDocumentInJournal()
    {
        return libfptr_find_document_in_journal(m_handle);
    }

    public int runFnCommand()
    {
        return libfptr_run_fn_command(m_handle);
    }
}
