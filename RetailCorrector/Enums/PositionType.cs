namespace RetailCorrector.Enums
{
    /// <summary>
    /// Тип позиции
    /// </summary>
    public enum PositionType : byte
    {
        /// <summary>
        /// Товар
        /// </summary>
        Product = 1,
        /// <summary>
        /// Проакцизированный товар
        /// </summary>
        Excise = 2,
        /// <summary>
        /// Выполняемая работа
        /// </summary>
        Work = 3,
        /// <summary>
        /// Оказание услуги
        /// </summary>
        Service = 4,
        /// <summary>
        /// Ставка в азартной игре
        /// </summary>
        Gambling = 5,
        /// <summary>
        /// Выйгрыш в азартной игре
        /// </summary>
        GamblingWin = 6,
        /// <summary>
        /// Ставка лотереи
        /// </summary>
        Lottery = 7,
        /// <summary>
        /// Выйгрыш лотереи
        /// </summary>
        LotteryWin = 8,
        /// <summary>
        /// Результат интеллектуальной деятельности
        /// </summary>
        IntellectualProperty = 9,
        /// <summary>
        /// Аванс/Преоплата/Кредит
        /// </summary>
        Payment = 10,
        /// <summary>
        /// Вознаграждение платёжному или иному агенту
        /// </summary>
        Agent = 11,
        /// <summary>
        /// Взнос в счет оплаты, пени, штрафе, вознаграждении, бонусе и др.
        /// </summary>
        Forfeit = 12,
        /// <summary>
        /// Другое
        /// </summary>
        Other = 13,
        /// <summary>
        /// Передача имущественного права
        /// </summary>
        PropertyLaw = 14,
        /// <summary>
        /// Внереализационный доход
        /// </summary>
        NonOperatingIncome = 15,
        /// <summary>
        /// Расход, указанный подпунктах 2 и 3 пункта Налогового кодекса РФ, уменьшающий сумму налога
        /// </summary>
        OtherPayment = 16,
        /// <summary>
        /// Торговый сбор
        /// </summary>
        TraceFee = 17,
        /// <summary>
        /// Курортный сбор
        /// </summary>
        ResortFee = 18,
        /// <summary>
        /// Залог
        /// </summary>
        Deposit = 19,
        /// <summary>
        /// Расход, в соответствии со статьей 346.16 пункта Налогового кодекса РФ
        /// </summary>
        Outcome = 20,
        /// <summary>
        /// Обязательное пенсионное страхование для ИП
        /// </summary>
        PensionInsuranceIE = 21,
        /// <summary>
        /// Обязательное пенсионное страхование для ИП
        /// </summary>
        PensionInsurance = 22,
        /// <summary>
        /// Обязательное медицинское страхование для ИП
        /// </summary>
        MedicalInsuranceIE = 23,
        /// <summary>
        /// Обязательное медицинское страхование
        /// </summary>
        MedicalInsurance = 24,
        /// <summary>
        /// Обязательное социальное страхование
        /// </summary>
        SocialInsurance = 25,
        /// <summary>
        /// Казино
        /// </summary>
        Casino = 26,
        #region ФФД 1.2
        /// <summary>
        /// Выдача денежных средств банковским платёжным агентом
        /// </summary>
        /// <remarks>Используется в ФФД 1.2</remarks>
        FromBankAgent = 27,
        /// <summary>
        /// Проакцизированный товар, подлежащий маркировке, не имеющий кода маркировки
        /// </summary>
        /// <remarks>Используется в ФФД 1.2</remarks>
        ExciseNonMark = 30,
        /// <summary>
        /// Проакцизированный товар, подлежащий маркировке, имеющий кода маркировки
        /// </summary>
        /// <remarks>Используется в ФФД 1.2</remarks>
        ExciseMark = 31,
        /// <summary>
        /// Товар, подлежащий маркировке, не имеющий кода маркировки
        /// </summary>
        /// <remarks>Используется в ФФД 1.2</remarks>
        NonMark = 32,
        /// <summary>
        /// Товар, подлежащий маркировке, имеющий кода маркировки
        /// </summary>
        /// <remarks>Используется в ФФД 1.2</remarks>
        Mark = 33,
        #endregion
    }
}