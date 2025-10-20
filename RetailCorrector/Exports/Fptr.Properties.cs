using Kraig.Attributes;

namespace RetailCorrector.Exports
{
    internal partial class Fptr
    {
        [NotifyChanged] private int _corrType  = 0;

        /// <summary>
        /// Печатать, ли чеки? (0 - Да; 1 - Нет)
        /// </summary>
        [NotifyChanged] private int _print = 1;

        [NotifyChanged] private string _actId = "";

        /// <summary>
        /// ФФД (0 - 1.2; 1 - 1.1; 2 - 1.05)
        /// </summary>
        [NotifyChanged] private int _fiscalFormat = 0;

        /// <summary>
        /// Причина коррекции на ФФД 1.05<br/>
        /// 0 - ошибка в реквизитах<br/>
        /// 1 - чек не был пробит ранее
        /// </summary>
        [NotifyChanged] private int _oldCorrReason = 0;
    }
}
