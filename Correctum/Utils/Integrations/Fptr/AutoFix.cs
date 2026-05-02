using System.Collections.Immutable;

namespace Correctum.Utils.Integrations
{
    internal sealed partial class FptrHandle
    {
        public static readonly ImmutableArray<int> AutoFixWithAsk = [];
        public static readonly ImmutableArray<int> AutoFixWoutAsk = [137];

        private bool TryFixFull30Storage() => 
            CloseSession(true) && OpenSession(true);

        private bool TryFixError137()
        {
            SetInteger(65622, 17);
            if (QueryFN() && GetInteger(65818) == 0) return TryFixFull30Storage();
            return false;
        }

        public bool TryFixErrorWithAsk(int err) => err switch
        {
            _ => false
        };

        public bool TryFixErrorWoutAsk(int err) => err switch
        {
            137 => TryFixError137(),
            _ => false
        };
    }
}
