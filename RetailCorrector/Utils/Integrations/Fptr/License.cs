namespace RetailCorrector.Utils.Integrations;

partial class FptrHandle
{
    public readonly struct License
    {
        public bool Entered { get; init; }
        public DateTime ValidFrom { get; init; }
        public DateTime ValidUntil { get; init; }
    }
    
    public readonly struct DetailLicense
    {
        public bool IsValid { get; init; }
        public string Name { get; init; }
        public string MaxConfigVersion { get; init; }
    }

    public License? GetLicenseInfo(uint number)
    {
        SetInteger(65587, 4);
        SetInteger(65610, number);
        if(!Query()) return null;
        return new License
        {
            Entered = GetBoolean(65611),
            ValidFrom = GetDate(65824),
            ValidUntil = GetDate(65825) 
        };
    }

    public uint? GetCountLicenses(uint number)
    {
        SetInteger(65587, 4);
        SetInteger(65610, number);
        SetInteger(65933, 0);
        return Query() ? GetInteger(65614) : null;
    }

    public DetailLicense? GetDetailLicenseInfo(uint number, uint index = 0)
    {
        SetInteger(65587, 4);
        SetInteger(65610, number);
        SetInteger(65933, index);
        if (!Query()) return null;
        return new DetailLicense
        {
            IsValid = GetBoolean(65934),
            Name = GetString(65821),
            MaxConfigVersion = GetString(65604)
        };
    }
}