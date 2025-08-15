namespace RetailCorrector.Data
{
    internal readonly struct ParseInfo(string name, params (string, object)[] args)
    {
        public Guid Id { get; } = Guid.CreateVersion7();
        public string Name { get; } = name;
        public DateTime Parsed { get; } = DateTime.Now;
        public (string, object)[] Arguments { get; } = args;

        public override string ToString() => $"Выгрузка {Name} от {Parsed:yyyy'-'MM'-'dd' 'HH':'mm':'ss} {ArgumentsToString()}";

        private string ArgumentsToString()
        {
            if (Arguments is null || Arguments.Length == 0) return "";
            var items = new string[Arguments.Length];
            for(var i = 0; i < items.Length; i++)
                items[i] = $"{Arguments[i].Item1}: {Arguments[i].Item2}";
            return $"({string.Join(", ", items)})";
        }
    }
}
