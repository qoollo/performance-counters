using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Базовый класс счётчика
    /// </summary>
    public abstract class Counter
    {
        /// <summary>
        /// Значение счётчика в случае ошибки (для числовых)
        /// </summary>
        public const long FailureNum = long.MinValue;
        /// <summary>
        /// Значение счётчика в случае ошибки (для временных)
        /// </summary>
        public readonly TimeSpan FailureTime = TimeSpan.MinValue;

        /// <summary>
        /// Конструктор Counter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="type">Тип счётчика</param>
        protected Counter(string name, string description, CounterTypes type)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            Name = name;
            Description = description;
            Type = type;
        }

        /// <summary>
        /// Имя счётчика
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Описание счётчика
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Тип счётчика
        /// </summary>
        public CounterTypes Type { get; protected set; }

        /// <summary>
        /// Сброс значений счётчика
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Преобразование в строку с информацией
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[Name: {0}, CounterType: {1}]", Name, Type);
        }
    }
}
