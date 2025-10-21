using Kraig.Attributes;

namespace RetailCorrector.Parsers;

internal partial class Saby
{
    [NotifyChanged] private string _login;
    [NotifyChanged] private string _password;
    [NotifyChanged] private string _inn;
    [NotifyChanged] private string _device;
    [NotifyChanged] private string _storage;
    [NotifyChanged] private DateTime _fromDate;
    [NotifyChanged] private DateTime _toDate;
}