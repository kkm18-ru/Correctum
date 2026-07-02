namespace Correctum.Utils.Integrations;

partial class FptrHandle
{
    public ExchangeOfdStatus? GetExchangeOfdStatus()
    {
        SetInteger(65622, 1);
        if(!QueryFN()) return null;
        return new ExchangeOfdStatus
        {
            Flags = (ExchangeOfdStatus.StatusFlags)GetInteger(65640),
            UnsentCount = GetInteger(65625),
            FirstUnsentNumber = GetInteger(65598),
            FirstUnsentDateTime = GetDate(65590),
            OkpDateTime = GetDate(65892),
            HasOfdMessages = GetBoolean(65691),
        }; 
    }

    public readonly struct ExchangeOfdStatus
    {
        [Flags] public enum StatusFlags
        {
            Connected = 1 << 0,
            HasMessages = 1 << 1,
            WaitingForResponse = 1 << 2,
            HasOfdCommand = 1 << 3,
            HasChangedOfdSettings = 1 << 4,
            WaitingForCommandResponse = 1 << 5
        }

        public StatusFlags Flags { get; init; }
        public uint UnsentCount { get; init; }
        public uint FirstUnsentNumber { get; init; }
        public bool HasOfdMessages { get; init; }
        public DateTime FirstUnsentDateTime { get; init; }
        public DateTime OkpDateTime { get; init; }
    }
}
