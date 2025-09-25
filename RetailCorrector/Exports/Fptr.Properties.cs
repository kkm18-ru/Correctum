using RetailCorrector.Attributes;

namespace RetailCorrector.Exports
{
    internal partial class Fptr
    {
        [NotifyChanged(DefaultValue = 0)] 
        public partial int CorrType { get; set; }
        /// <summary>
        /// Печатать, ли чеки? (0 - Да; 1 - Нет)
        /// </summary>
        [NotifyChanged(DefaultValue = 1)] 
        public partial int Print { get; set; }
        [NotifyChanged(DefaultValue = "")] 
        public partial string ActId { get; set; }
        /// <summary>
        /// ФФД (0 - 1.2; 1 - 1.1; 2 - 1.05)
        /// </summary>
        [NotifyChanged(DefaultValue = 0)] 
        public partial int FiscalFormat { get; set; }
        /// <summary>
        /// Причина коррекции на ФФД 1.05<br/>
        /// 0 - ошибка в реквизитах<br/>
        /// 1 - чек не был пробит ранее
        /// </summary>
        [NotifyChanged(DefaultValue = 0)] 
        public partial int OldCorrReason { get; set; }
    }
}
