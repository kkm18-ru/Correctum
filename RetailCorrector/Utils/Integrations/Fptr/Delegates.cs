using System.Runtime.InteropServices;

namespace RetailCorrector.Utils.Integrations
{
    partial class FptrHandle
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int HandleManagementDelegate(out nint handle);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SimpleDelegate(nint handle);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool GetBoolDelegate(nint handle, int paramId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetBoolDelegate(nint handle, int paramId, bool value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate uint GetIntDelegate(nint handle, int paramId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetIntDelegate(nint handle, int paramId, uint value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate double GetDoubleDelegate(nint handle, int paramId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetDoubleDelegate(nint handle, int paramId, double value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetStrDelegate(nint handle, int paramId, byte[] value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetBytesDelegate(nint handle, int paramId, byte[] value, int size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetBytesDelegate(nint handle, int paramId, byte[] value, int size);

#pragma warning disable S107
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetDateDelegate(nint handle, int paramId, int year, int month, int day, int hour, int minute, int second);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void GetDateDelegate(nint handle, int paramId, out nint year, out nint month, out nint day, out nint hour, out nint minute, out nint second);
#pragma warning restore S107

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetErrorDescriptionDelegate(nint handle, byte[] value, int size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int ShowPropertiesDelegate(nint handle, int parentType, nint parent);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetSettingDelegate(nint handle, byte[] key, byte[] value, int size);


        private static T GetDelegateOfLib<T>(string nameMethod)
            where T : Delegate
        {
            var proc = NativeLibrary.GetExport(hLibrary, nameMethod);
            return Marshal.GetDelegateForFunctionPointer<T>(proc);
        }
    }
}
