using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Correctum.Utils.Extensions;

public static partial class ColorExtensions
{
    extension(Color)
    {
        public static Color FromHex(string hex)
        {
            if (!HexCodeColorRegex().IsMatch(hex))
                throw new ArgumentException("Ввод не является HEX-кодом цвета", nameof(hex));
            if (hex.Length == 4)
            {
                var builder = new StringBuilder(hex);
                for(var i = 3; i > 0; i--)
                    builder.Insert(i, builder[i]);
                hex = builder.ToString();
            }
            Span<byte> bytes = stackalloc byte[3];
            Convert.FromHexString(hex[1..], bytes, out _, out _);
            return Color.FromRgb(bytes[0], bytes[1], bytes[2]);
        }
    }

    [GeneratedRegex(@"^#(([0-9a-fA-F]{2}){3}|([0-9a-fA-F]){3})$")]
    private static partial Regex HexCodeColorRegex();
}
