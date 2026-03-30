using System.Runtime.InteropServices;
using System.Text;

namespace Correctum.Fptr10;

public class OSDetector
{
    public enum OSType
    {
        None,
        Unknown,
        Linux,
        Windows,
        OSX
    }

    private static bool _initialized = false;

    private static int _symbolSize = -1;

    private static OSType _os = OSType.None;

    private static Encoding _textEncoding = Encoding.Default;

    public static OSType Type
    {
        get
        {
            if (!_initialized)
            {
                Initialize();
            }

            return _os;
        }
    }

    public static int SymbolSize
    {
        get
        {
            if (!_initialized)
            {
                Initialize();
            }

            return _symbolSize;
        }
    }

    public static Encoding Encoding
    {
        get
        {
            if (!_initialized)
            {
                Initialize();
            }

            return _textEncoding;
        }
    }

    private static void Initialize()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _os = OSType.Windows;
            _symbolSize = 2;
            _textEncoding = Encoding.Unicode;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            _os = OSType.Linux;
            _symbolSize = 4;
            _textEncoding = Encoding.UTF32;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            _os = OSType.OSX;
            _symbolSize = 4;
            _textEncoding = Encoding.UTF32;
        }
        else
        {
            _os = OSType.Unknown;
            _symbolSize = 4;
            _textEncoding = Encoding.UTF32;
        }

        _initialized = true;
    }
}
