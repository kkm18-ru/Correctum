namespace Correctum.Enums
{
    /// <summary>
    /// Указывает меру количества предмета расчета
    /// </summary>
    public enum UnitOfMeasure : byte
    {
        /// <summary>
        /// Штук/Единиц
        /// </summary>
        Piece = 0,
        /// <summary>
        /// Грам
        /// </summary>
        Gram = 10,
        /// <summary>
        /// Килограм
        /// </summary>
        Kilogram = 11,
        /// <summary>
        /// Тонна
        /// </summary>
        Ton = 12,
        /// <summary>
        /// Сентиметр
        /// </summary>
        Centimeter = 20,
        /// <summary>
        /// Дециметр
        /// </summary>
        Decimeter = 21,
        /// <summary>
        /// Метр
        /// </summary>
        Meter = 22,
        /// <summary>
        /// Квадратный Сентиметр
        /// </summary>
        SquareCentimeter = 30,
        /// <summary>
        /// Квадратный Дециметр
        /// </summary>
        SquareDecimeter = 31,
        /// <summary>
        /// Квадратный Метр
        /// </summary>
        SquareMeter = 32,
        /// <summary>
        /// Миллилитр
        /// </summary>
        Milliliter = 40,
        /// <summary>
        /// Литр
        /// </summary>
        Liter = 41,
        /// <summary>
        /// Кубический Метр
        /// </summary>
        CubicMeter = 42,
        /// <summary>
        /// Киловатт * час
        /// </summary>
        KilowattHour = 50,
        /// <summary>
        /// Гигакалорий
        /// </summary>
        Gigacalories = 51,
        /// <summary>
        /// Дней
        /// </summary>
        Days = 70,
        /// <summary>
        /// Часов
        /// </summary>
        Hours = 71,
        /// <summary>
        /// Минут
        /// </summary>
        Minutes = 72,
        /// <summary>
        /// Секунд
        /// </summary>
        Seconds = 73,
        /// <summary>
        /// Килобайт
        /// </summary>
        Kilobyte = 80,
        /// <summary>
        /// Мегабайт
        /// </summary>        
        Megabyte = 81,
        /// <summary>
        /// Гигабайт
        /// </summary>
        Gigabyte = 82,
        /// <summary>
        /// Терабайт
        /// </summary>
        Terabyte = 83,
        /// <summary>
        /// Другое
        /// </summary>
        Other = 255
    }
}