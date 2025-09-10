using System.Windows.Controls;
using System.Windows.Data;

namespace RetailCorrector.Utils
{
    internal class ValidatedBinding : Binding
    {
        public ValidatedBinding(string path, ValidationRule validRule) : base(path)
        {
            NotifyOnValidationError = true;
            ValidationRules.Add(validRule);
        }
    }
}
