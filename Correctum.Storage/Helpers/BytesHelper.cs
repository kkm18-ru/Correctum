using Correctum.Storage.Extensions;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace Correctum.Storage.Helpers
{
    internal static class BytesHelper
    {
        public static void Write(Span<byte> bytes, int offset, int length, ReadOnlySpan<char> chars) =>
            Encoding.CorrectumStorage.GetBytes(chars, bytes.Slice(offset, length));
        public static void Write(Span<byte> bytes, int offset, decimal value) =>
            MemoryMarshal.Write(bytes.Slice(offset, sizeof(decimal)), value);

        public static void Write(Span<byte> bytes, int offset, long value) =>
            BinaryPrimitives.WriteInt64LittleEndian(bytes.Slice(offset, sizeof(long)), value);

        public static void Write(Span<byte> bytes, int offset, int value) =>
            BinaryPrimitives.WriteInt32LittleEndian(bytes.Slice(offset, sizeof(int)), value);

        public static void Write(Span<byte> bytes, int offset, short value) =>
            BinaryPrimitives.WriteInt16LittleEndian(bytes.Slice(offset, sizeof(short)), value);

        public static void Write(Span<byte> bytes, int offset, ulong value) =>
            BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(offset, sizeof(ulong)), value);

        public static void Write(Span<byte> bytes, int offset, uint value) =>
            BinaryPrimitives.WriteUInt32LittleEndian(bytes.Slice(offset, sizeof(uint)), value);

        public static void Write(Span<byte> bytes, int offset, ushort value) =>
            BinaryPrimitives.WriteUInt16LittleEndian(bytes.Slice(offset, sizeof(ushort)), value);

        public static void Write(Span<byte> bytes, int offset, byte value) => bytes[offset] = value;

        public static string ReadString(ReadOnlySpan<byte> bytes, int offset, int length) =>
            Encoding.CorrectumStorage.GetString(bytes.Slice(offset, length));

        public static decimal ReadDecimal(ReadOnlySpan<byte> bytes, int offset) =>
            MemoryMarshal.Read<decimal>(bytes.Slice(offset, sizeof(decimal)));

        public static ulong ReadUInt64(ReadOnlySpan<byte> bytes, int offset) =>
            BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(offset, sizeof(ulong)));

        public static uint ReadUInt32(ReadOnlySpan<byte> bytes, int offset) =>
            BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(offset, sizeof(uint)));

        public static ushort ReadUInt16(ReadOnlySpan<byte> bytes, int offset) =>
            BinaryPrimitives.ReadUInt16LittleEndian(bytes.Slice(offset, sizeof(ushort)));

        public static byte ReadUInt8(ReadOnlySpan<byte> bytes, int offset) => bytes[offset];

        public static long ReadInt64(ReadOnlySpan<byte> bytes, int offset) =>
            BinaryPrimitives.ReadInt64LittleEndian(bytes.Slice(offset, sizeof(long)));

        public static int ReadInt32(ReadOnlySpan<byte> bytes, int offset) =>
            BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(offset, sizeof(int)));

        public static short ReadInt16(ReadOnlySpan<byte> bytes, int offset) =>
            BinaryPrimitives.ReadInt16LittleEndian(bytes.Slice(offset, sizeof(short)));
    }
}
