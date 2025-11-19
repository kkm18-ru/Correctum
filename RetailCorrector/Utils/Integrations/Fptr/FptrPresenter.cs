using System.Windows;
using Microsoft.Win32;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.IO;
using System.Text;

namespace RetailCorrector.Utils.Integrations.Fptr
{
    internal static class FptrPresenter
    {
        public static uint? GetSessionState(nint handle)
        {
            FptrNative.libfptr_set_param_int(handle, 65587, 14);
            if(FptrNative.libfptr_query_data(handle) != 0) return null;
            return FptrNative.libfptr_get_param_int(handle, 65592);
        }

        public static uint? GetFFD(nint handle)
        {
            FptrNative.libfptr_set_param_int(handle, 65622, 7);
            if(FptrNative.libfptr_fn_query_data(handle) != 0) return null;
            var fn = FptrNative.libfptr_get_param_int(handle, 65628);
            var dev = FptrNative.libfptr_get_param_int(handle, 65629);
            return (fn, dev) switch
            {
                (0, _) => dev, (_, 0) => fn,
                (_, _) => Math.Min(fn, dev),
            };
        }

        public static bool AddPosition(nint handle, Position pos, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var ffd = GetFFD(handle);
            FptrNative.libfptr_set_param_str(handle, 65631, pos.Name);
            FptrNative.libfptr_set_param_double(handle, 65632, pos.Price / 100.0);
            FptrNative.libfptr_set_param_double(handle, 65633, pos.Quantity / 1000.0);
            FptrNative.libfptr_set_param_double(handle, 65634, pos.Cost / 100.0);
            FptrNative.libfptr_set_param_int(handle, 65569, pos.TaxRate.ToAtol());
            FptrNative.libfptr_set_param_int(handle, 1212, (uint)pos.Type);
            FptrNative.libfptr_set_param_int(handle, 1214, (uint)pos.PayMethod);
            if(ffd == 120) 
                FptrNative.libfptr_set_param_int(handle, 2108, (uint)pos.MeasureUnit!);
            return 0 == FptrNative.libfptr_registration(handle);
        }

        public static bool Payment(nint handle, Payment pay, CancellationToken token) =>
            SinglePayment(handle, 0, pay.Cash, token) && 
            SinglePayment(handle, 1, pay.ECash, token) &&
            SinglePayment(handle, 2, pay.Pre, token) && 
            SinglePayment(handle, 3, pay.Post, token) &&
            SinglePayment(handle, 4, pay.Provision, token);

        private static bool SinglePayment(nint handle, uint type, ulong sum, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (sum == 0) return true;
            FptrNative.libfptr_set_param_int(handle, 65564, type);
            FptrNative.libfptr_set_param_double(handle, 65565, sum/100.0);
            return 0 == FptrNative.libfptr_payment(handle);
        }

        public static bool TotalReceipt(nint handle, ulong total, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            FptrNative.libfptr_set_param_double(handle, 65613, total / 100.0);
            return 0 == FptrNative.libfptr_receipt_total(handle);
        }

        public struct OpenReceiptArgs
        {
            public bool IsInternetPayment { get; set; }
            public string HttpAddress { get; set; }
            public string Act { get; set; }
            public bool Print { get; set; }
            public int CorrType { get; set; }
        }
        
        public static bool OpenReceipt(nint handle, Receipt doc, OpenReceiptArgs args, CancellationToken token)
        {
            const int DefaultCorrDataSize = 128;
            token.ThrowIfCancellationRequested();
            var ffd = GetFFD(handle);
            var corrData = new byte[DefaultCorrDataSize];

            if(ffd >= 110)  // todo add for ffd 1.05
            {
                FptrNative.libfptr_set_param_datetime(handle, 1178, 
                    doc.Created.Year, doc.Created.Month, doc.Created.Day, 
                    doc.Created.Hour, doc.Created.Minute, doc.Created.Second);
                FptrNative.libfptr_set_param_str(handle, 1179, args.Act);
                if (FptrNative.libfptr_util_form_tlv(handle) != 0) return false;
                var count = FptrNative.libfptr_get_param_bytearray(handle, 65624, corrData, DefaultCorrDataSize);
                if (count != DefaultCorrDataSize)
                {
                    Array.Resize(ref corrData, count);
                    if(count > DefaultCorrDataSize)
                        FptrNative.libfptr_get_param_bytearray(handle, 65624, corrData, count);
                }
            }
            token.ThrowIfCancellationRequested();

            var opcode = GetOperation(handle, doc.Type);
            if (opcode is null) return false;
            FptrNative.libfptr_set_param_int(handle, 65545, (uint)opcode);
            FptrNative.libfptr_set_param_bool(handle, 65572, args.Print);

            if (args.IsInternetPayment)
            {
                FptrNative.libfptr_set_param_bool(handle, 1125, true);
                FptrNative.libfptr_set_param_str(handle, 1008, doc.BuyerContact);
                FptrNative.libfptr_set_param_str(handle, 1187, args.HttpAddress);
            }

            token.ThrowIfCancellationRequested();
            if (ffd >= 110) // todo add for ffd 1.05
            {
                FptrNative.libfptr_set_param_int(handle, 1173, (uint)args.CorrType);
                FptrNative.libfptr_set_param_bytearray(handle, 1174, corrData, corrData.Length);
            }
            FptrNative.libfptr_set_param_str(handle, 1192, doc.DocSign.ToString());
            return 0 == FptrNative.libfptr_open_receipt(handle);
        }

        public static void CancelReceipt(nint handle)
        {
            var code = FptrNative.libfptr_error_code(handle);
            var desc = GetErrorDescription(handle);
            FptrNative.libfptr_cancel_receipt(handle);
            var title = $"Произошла ошибка (Код: {code})";
            if (code == 137)
            {
                FptrNative.libfptr_set_param_int(handle, 65622, 17);
                if(FptrNative.libfptr_fn_query_data(handle) == 0 &&       // Получение свободного места
                    FptrNative.libfptr_get_param_int(handle, 65818) == 0) // 30-дневого хранилища (в КБ)
                {
                    var msgResult = MessageBox.Show("Выполнить автоматическое исправление ошибки?", title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (msgResult == MessageBoxResult.Yes)
                    {
                        FptrNative.libfptr_set_param_int(handle, 65546, 0);
                        if (0 != FptrNative.libfptr_report(handle) || 0 != FptrNative.libfptr_open_shift(handle))
                            return;
                        var old = code;
                        code = FptrNative.libfptr_error_code(handle);
                        title = $"Произошла ошибка (Коды: {old}, {code})";
                        MessageBox.Show(
                            $"Во время попытки исправления ошибки возникла новая (Код: {code})",
                            "Автоматическое исправление не удалось!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
            FptrNative.libfptr_close(handle);
            var text = $"{desc}\n\nДрайвер освободил кассу для её настройки, закройте данное окно для продолжения пробития!";
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Hand);
            FptrNative.libfptr_open(handle);
        }

        private static int? GetOperation(nint handle, Operation opcode) => (GetFFD(handle), opcode) switch
        {
            (null, _) => null,
            (<= 105, Operation.Income or Operation.RefundOutcome) => 1,
            (<= 105, Operation.Outcome or Operation.RefundIncome) => 4,
            (>= 110, Operation.Income) => 7,
            (>= 110, Operation.Outcome) => 9,
            (>= 110, Operation.RefundOutcome) => 10,
            (>= 110, Operation.RefundIncome) => 8,
            _ => throw new NotImplementedException()
        };

        private static string GetErrorDescription(nint handle)
        {
            StringBuilder stringBuilder = new(128);
            int num = FptrNative.libfptr_error_description(handle, stringBuilder, stringBuilder.Capacity);
            if (num > stringBuilder.Capacity)
            {
                stringBuilder.EnsureCapacity(num);
                FptrNative.libfptr_error_description(handle, stringBuilder, stringBuilder.Capacity);
            }

            return stringBuilder.ToString();
        }

        public static nint LoadLibrary(string path)
        {
            var intPtr = FptrNative.LoadLibrary(path);
            if (intPtr == IntPtr.Zero)
            {
                intPtr = FptrNative.LoadLibrary(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "msvcp140.dll"));
                if (intPtr == IntPtr.Zero) throw new Exception("Не удалось загрузить msvcp140.dll");
                intPtr = FptrNative.LoadLibrary(path);
                if (intPtr == IntPtr.Zero) throw new Exception("Не удалось загрузить драйвер Fptr");
            }
            return intPtr;
        }

        public static string GetLocationDriver() =>
            Path.Combine((string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\ATOL\Drivers\10.0\KKT",
                "INSTALL_DIR", "")!, "bin", "fptr10.dll");

        public static bool LoginOperator(nint handle, string name, string inn)
        {
            FptrNative.libfptr_set_param_str(handle, 1021, name);
            FptrNative.libfptr_set_param_str(handle, 1203, inn);
            return FptrNative.libfptr_operator_login(handle) == 0;
        }

        public static bool WaitReboot(nint handle)
        {
            const int Delay = 100;
            var countInteration = Configuration.Instance.FptrWaitRebootTimeout / Delay;
            for (var _ = 0; _ < countInteration; _++)
            {
                FptrNative.libfptr_set_param_int(handle, 65587, 16); // get serial number
                if (FptrNative.libfptr_query_data(handle) == 0) return true;
                Thread.Sleep(Delay);
            }
            return false;
        }

        public static bool SilentReboot(nint handle)
        {
            FptrNative.libfptr_set_param_bool(handle, 65873, false);
            return FptrNative.libfptr_device_reboot(handle) == 0;
        }
    }
}
