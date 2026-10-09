using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Enums;
using Correctum.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Correctum.Actions;

//[ActionInfo("Пометить по ставке НДС")]
[CheckBox("Ставка НДС может отсутствовать", nameof(IsNullTaxRate))]
[LocalizedComboBox("Ставка НДС", nameof(TaxRate), nameof(Utils.EnumLocales.TaxRates))]
public sealed class MarkByTaxRateChanger : BaseChanger
{
    public bool IsNullTaxRate { get; set; }
    public NDSRate TaxRate { get; set; }

    public override Expression<Func<Receipt, bool>> Filter =>
        r => r.Items.Any(p => IsNullTaxRate ? p.TaxRate == null : p.TaxRate!.Value == TaxRate);

    public override void Update(UpdateSettersBuilder<Receipt> builder) =>
        builder.SetProperty(r => r.Marked, true);
}
