using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Интерфейс счётчика времени
    /// </summary>
    public interface ITimeCounter
    {
        /// <summary>
        /// Зарегистрировать измерение времени
        /// </summary>
        /// <param name="period">Период времени</param>
        void RegisterMeasurement(TimeSpan period);
    }


    /// <summary>
    /// Таймер для счётчика среднего времени
    /// </summary>
    public struct TimeCounterTimer: IDisposable
    {
        /// <summary>
        /// Создать и запустить таймер
        /// </summary>
        /// <param name="sourceCounter">Счётчик среднего времени</param>
        /// <returns>Запущенный таймер</returns>
        public static TimeCounterTimer StartNew(ITimeCounter sourceCounter)
        {
            var result = new TimeCounterTimer(sourceCounter);
            result.Start();
            return result;
        }


        private readonly ITimeCounter _srcCounter;
        private Stopwatch _timer;

        /// <summary>
        /// Конструктор AverageTimeCounterTimer
        /// </summary>
        /// <param name="sourceCounter">Счётчик среднего времени</param>
        public TimeCounterTimer(ITimeCounter sourceCounter)
        {
            if (sourceCounter == null)
                throw new ArgumentNullException("sourceCounter");

            _srcCounter = sourceCounter;
            _timer = new Stopwatch();
        }

        /// <summary>
        /// Текущее время
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                if (_timer == null)
                    return TimeSpan.Zero;
                return _timer.Elapsed;
            }
        }

        /// <summary>
        /// Запустить
        /// </summary>
        public void Start()
        {
            if (_timer != null)
                _timer.Start();
            else if (_srcCounter != null)
                _timer = Stopwatch.StartNew();
        }

        /// <summary>
        /// Поставить на паузу
        /// </summary>
        public void Pause()
        {
            if (_timer != null)
                _timer.Stop();
        }

        /// <summary>
        /// Перезапустить
        /// </summary>
        public void Restart()
        {
            if (_timer != null)
                _timer.Restart();
        }

        /// <summary>
        /// Зафиксировать текущее измерение в счётчике
        /// </summary>
        public void Complete()
        {
            if (_timer != null)
            {
                _timer.Stop();
                if (_srcCounter != null)
                    _srcCounter.RegisterMeasurement(_timer.Elapsed);
            }
        }

        /// <summary>
        /// Остановка таймера и фиксация измерения
        /// </summary>
        void IDisposable.Dispose()
        {
            Pause();
            Complete();
        }


        /// <summary>
        /// Преобразование в строку с информацией
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[Elapsed: {0}]", Elapsed);
        }
    }
}
