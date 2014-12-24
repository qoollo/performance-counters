using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Счётчик среднего времени
    /// </summary>
    public abstract class AverageTimeCounter: Counter, ITimeCounter
    {
        /// <summary>
        /// Конструктор AverageTimeCounter
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="description">Описание</param>
        protected AverageTimeCounter(string name, string description)
            : base(name, description, CounterTypes.AverageTime)
        {
            IsMeasurementEnabled = true;
        }

        /// <summary>
        /// Включено ли измерение времени
        /// </summary>
        public bool IsMeasurementEnabled { get; set; }
        /// <summary>
        /// Отключить измерение времени
        /// </summary>
        public void DisableMeasurement()
        {
            IsMeasurementEnabled = false;
        }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public abstract TimeSpan CurrentValue { get; }

        /// <summary>
        /// Запуск нового таймера
        /// </summary>
        /// <returns>Таймер</returns>
        public TimeCounterTimer StartNew()
        {
            if (IsMeasurementEnabled)
                return TimeCounterTimer.StartNew(this);
            else
                return new TimeCounterTimer();
        }

        /// <summary>
        /// Зарегистрировать измерение
        /// </summary>
        /// <param name="period">Период</param>
        public abstract void RegisterMeasurement(TimeSpan period);


        /// <summary>
        /// Сброс значений счётчика
        /// </summary>
        public override void Reset()
        {
        }


        /// <summary>
        /// Преобразование в строку с информацией
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[Value: {0}, Name: {1}, CounterType: {2}]", CurrentValue, Name, Type);
        }
    }
}
