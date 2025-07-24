namespace RetailCorrector.Enums
{
    /// <summary>
    /// Единица измерения
    /// </summary>
    public enum MeasureUnit : byte
    {
        /// <summary>
        /// Штук/Единиц
        /// </summary>
        Default = 0,
        /// <summary>
        /// Грам
        /// </summary>
        g = 10,
        /// <summary>
        /// Килограм
        /// </summary>
        kg = 11,
        /// <summary>
        /// Тонна
        /// </summary>
        t = 12,
        /// <summary>
        /// Сентиметр
        /// </summary>
        cm = 20,
        /// <summary>
        /// Дециметр
        /// </summary>
        dm = 21,
        /// <summary>
        /// Метр
        /// </summary>
        m = 22,
        /// <summary>
        /// Квадратный Сентиметр
        /// </summary>
        cm2 = 30,
        /// <summary>
        /// Квадратный Дециметр
        /// </summary>
        dm2 = 31,
        /// <summary>
        /// Квадратный Метр
        /// </summary>
        m2 = 32,
        /// <summary>
        /// Миллилитр
        /// </summary>
        mL = 40,
        /// <summary>
        /// Литр
        /// </summary>
        L = 41,
        /// <summary>
        /// Кубический Метр
        /// </summary>
        m3 = 42,
        /// <summary>
        /// Киловатт * час
        /// </summary>
        kW_h = 50,
        /// <summary>
        /// Гигакалорий
        /// </summary>
        GCal = 51,
        /// <summary>
        /// Дней
        /// </summary>
        Day = 70,
        /// <summary>
        /// Часов
        /// </summary>
        h = 71,
        /// <summary>
        /// Минут
        /// </summary>
        min = 72,
        /// <summary>
        /// Секунд
        /// </summary>
        sec = 73,
        /// <summary>
        /// Килобайт
        /// </summary>
        kByte = 80,
        /// <summary>
        /// Мегабайт
        /// </summary>        
        MByte = 81,
        /// <summary>
        /// Гигабайт
        /// </summary>
        GByte = 82,
        /// <summary>
        /// Терабайт
        /// </summary>
        TByte = 83,
        /// <summary>
        /// Другое
        /// </summary>
        Other = 255
    }
}