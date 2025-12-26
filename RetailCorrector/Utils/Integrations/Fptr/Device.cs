using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace RetailCorrector.Utils.Integrations
{
    internal sealed partial class FptrHandle
    {
        public int ErrorCode => errorCode(handle);
        public string ErrorDescription
        {
            get
            {
                var value = new byte[defaultStringSize * 2];
                var count = errorDescription(handle, value, defaultStringSize);
                if (count > defaultStringSize)
                {
                    value = new byte[count * 2];
                    errorDescription(handle, value, count);
                }
                return Encoding.Unicode.GetString(value, 0, count * 2).Trim('\0', ' ', '\t', '\n', '\r');
            }
        }

        public uint? DocFormat => docFormat;

        public bool Connect()
        {
            if (open(handle) == 0)
            {
                SetInteger(65622, 7);
                if (QueryFN())
                {
                    var fn = GetInteger(65628);
                    var dev = GetInteger(65629);
                    docFormat = (fn, dev) switch
                    {
                        (0, _) => dev,
                        (_, 0) => fn,
                        (_, _) => Math.Min(fn, dev),
                    };
                }
                return true;
            }
            return false;
        }
        public bool Disconnect()
        {
            docFormat = null;
            return close(handle) == 0;
        }

        public bool ShowProperties(Window wind) =>
            showProperties(handle, 0, new WindowInteropHelper(wind).Handle) == 0;

        public bool LoginOperator(string fullname, string inn = "")
        {
            SetString(1021, fullname);
            SetString(1203, inn);
            return operatorLogin(handle) == 0;
        }
    }
}
