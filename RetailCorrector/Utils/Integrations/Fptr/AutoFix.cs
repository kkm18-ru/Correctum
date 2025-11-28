namespace RetailCorrector.Utils.Integrations
{
    internal sealed partial class FptrHandle
    {
        public bool TryFixFull30Storage() => 
            !CloseSession(true) || !OpenSession(true);

        public bool TryFixError137()
        {
            SetInteger(65622, 17);
            if (QueryFN() && GetInteger(65818) == 0) return TryFixFull30Storage();
            return false;
        }

        public bool TryFixError() => ErrorCode switch
        {
            137 => TryFixError137(),
            _ => false
        };
    }
}
