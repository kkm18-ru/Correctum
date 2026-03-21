namespace Correctum
{
    public static class Validator
    {
        public static string? ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Name cannot be empty.";
            if (name.Length < 3) return "Name must be at least 3 characters long.";
            return null;
        }
    }
}
