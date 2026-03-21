namespace Correctum.Models
{
    public sealed class Item
    {
        public int Id { get; init; }
        public string Name { get; set => Utils.SetField(ref field, value, Validator.ValidateName); }
        public int Value { get; set => Utils.SetField(ref field, value, ValidateValue); }

        private string? ValidateValue(int value)
        {
            if (value < 0) return "Value cannot be negative.";
            return null;
        }
    }
}
