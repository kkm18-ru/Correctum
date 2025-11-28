namespace RetailCorrector.Utils.Integrations
{
    internal sealed partial class FptrHandle
    {
        public bool ProcessSession(bool silent = false)
        {
            var currStatus = SessionStatus;
            if (currStatus is null) return false;
            if (currStatus == 2)
            {
                if (!CloseSession(silent)) return false;
                currStatus = SessionStatus;
            }
            if (currStatus == 0)
            {
                if (!OpenSession(silent)) return false;
                currStatus = SessionStatus;
            }
            return currStatus == 1;
        }

        public uint? SessionStatus
        {
            get
            {
                SetInteger(65587, 14);
                if (!Query()) return null;
                return GetInteger(65592);
            }
        }

        public bool OpenSession(bool silent = false)
        {
            SetBoolean(65749, silent);
            return openShift(handle) == 0;
        }

        public bool CloseSession(bool silent = false)
        {
            SetInteger(65546, 0);
            SetBoolean(65749, silent);
            return Report();
        }
    }
}
