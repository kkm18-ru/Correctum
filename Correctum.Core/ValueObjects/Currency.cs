using System.Globalization;

namespace Correctum.Core.ValueObjects
{
    /// <summary>
    /// Представляет денежное значение.
    /// </summary>
    public readonly struct Currency
    {
        public readonly static Currency Zero = new(0);

        private readonly ulong _data;

        private Currency(ulong data) => _data = data;

        /// <summary>
        /// Преобразует целочисленное число без знака в число <see cref="Currency"/>.
        /// </summary>
        /// <param name="value">Преобразуемое целое число без знака.</param>
        public static implicit operator Currency(ulong value) => new(value);

        /// <summary>
        /// Преобразует целочисленное число в число <see cref="Currency"/>.
        /// </summary>
        /// <param name="value">Преобразуемое целое число.</param>
        public static implicit operator Currency(long value)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            return new((ulong)value);
        }

        /// <summary>
        /// Преобразует десятичное число с плавающей точкой в число <see cref="Currency"/>.
        /// </summary>
        /// <param name="value">Преобразуемое десятичное число с плавающей точкой.</param>
        public static implicit operator Currency(decimal value)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            return new((ulong)Math.Round(value * 100));
        }

        /// <summary>
        /// Преобразует число с плавающей точкой в число <see cref="Currency"/>.
        /// </summary>
        /// <param name="value">Преобразуемое число с плавающей точкой.</param>
        public static implicit operator Currency(double value)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            return new((ulong)Math.Round(value * 100));
        }

        /// <summary>
        /// Преобразует число <see cref="Currency"/> в число с плавающей точкой.
        /// </summary>
        /// <param name="value">Преобразуемое число <see cref="Currency"/>.</param>
        public static explicit operator double(Currency value) => value._data / 100D;
        /// <summary>
        /// Преобразует число <see cref="Currency"/> в десятичное число с плавающей точкой.
        /// </summary>
        /// <param name="value">Преобразуемое число <see cref="Currency"/>.</param>
        public static explicit operator decimal(Currency value) => value._data / 100M;
        /// <summary>
        /// Преобразует число <see cref="Currency"/> в целочисленное число без знака.
        /// </summary>
        /// <param name="value">Преобразуемое число <see cref="Currency"/>.</param>
        public static explicit operator ulong(Currency value) => value._data;

        /// <summary>
        /// Возвращает строковое представление значения этого экземпляра <see cref="Currency"/> структуры.
        /// </summary>
        /// <returns>Значение этого <see cref="Currency"/>.</returns>
        public override string ToString() => ((decimal)this).ToString("F2", CultureInfo.InvariantCulture);
    }
}
