namespace RetailCorrector.Utils.Integrations
{
    partial class FptrHandle
    {
        private bool UtilFromTLV() => utilFormTLV(handle) == 0;
        private bool QueryFN() => fnQueryData(handle) == 0;
        private bool Query() => queryData(handle) == 0;
        private bool Report() => report(handle) == 0;

        private bool GetBoolean(int paramId) => getParamBool(handle, paramId);
        private uint GetInteger(int paramId) => getParamInt(handle, paramId);

        private void SetBoolean(int paramId, bool value) => setParamBool(handle, paramId, value);
        private void SetInteger(int paramId, uint value) => setParamInt(handle, paramId, value);
        private void SetDouble(int paramId, double value) => setParamDouble(handle, paramId, value);

        private void SetBytes(int paramId, byte[] value) =>
            setParamByteArray(handle, paramId, value, value.Length);

        private byte[] GetBytes(int paramId)
        {
            var value = new byte[defaultBytesSize];
            var count = getParamByteArray(handle, paramId, value, value.Length);
            if (defaultBytesSize != count)
            {
                value = new byte[count];
                getParamByteArray(handle, paramId, value, value.Length);
            }
            return value;
        }

        private void SetString(int paramId, string value) =>
            setParamStr(handle, paramId, encoding.GetBytes(value));

        private string GetString(int paramId)
        {
            var value = new byte[defaultStringSize * sizeCharacter];
            var count = getParamStr(handle, paramId, value, defaultStringSize);
            if (count > defaultStringSize)
            {
                value = new byte[count * sizeCharacter];
                getParamStr(handle, paramId, value, count);
            }
            return encoding.GetString(value, 0, count * sizeCharacter).TrimEnd();
        }

        private DateTime GetDate(int paramId)
        {
            getParamDateTime(handle, paramId, out var y, out var M, out var d, out var h, out var m, out var s);
            return new DateTime(y.ToInt32(), M.ToInt32(), d.ToInt32(), h.ToInt32(), m.ToInt32(), s.ToInt32(), (DateTimeKind)0);
        }

        private void SetDate(int paramId, DateTime value) =>
            setParamDateTime(handle, paramId, value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);

        private Version? GetConfigurationVersion()
        {
            SetInteger(65587, 2); // DataType: UnitVersion 
            SetInteger(65609, 1); // UnitType: Configuration 
            return !Query() ? null : Version.Parse(GetString(65604));
        }
    }
}
